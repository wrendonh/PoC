using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Crushbank.Common.Entities.Discovery;
using Crushbank.Common.Utilities;
using Crushbank.Core.Interfaces;
using Microsoft.AspNetCore.SignalR;
using PersistingPoC.Interfaces;
using PersistingPoC.Service.Dtos;
using PersistingPoC.Service.Dtos.ConnectWise;
using PersistingPoC.Service.Interfaces;
using PersistingPoC.Service.Interfaces.Sql;
using PersistingPoC.Service.Services;
using static PersistingPoC.Entities.Enums;
using ConnectWiseSDKServiceModel = ConnectWiseDotNetSDK.ConnectWise.Client.Service.Model;

namespace PersistingPoC.Integrations
{
    public class ConnectWiseTicketIntegration : IConnectWiseTicketIntegration
    {
        private readonly ITicketService _ticketService;
        private readonly ITaskTypeIntegrationService _taskTypeIntegrationService;
        private readonly ITaskProcessService _taskProcessService;
        private readonly Service.Interfaces.ConnectWise.ITicketService _ticketConnectWiseService;
        private readonly Service.Interfaces.ConnectWise.IUtilityService _utilityService;
        private readonly IWatsonContentService _watsonContentService;
        private readonly IHubContext<TicketHub, ITicketHub> _ticketHub;
        private readonly IRedisService _redisService;

        public ConnectWiseTicketIntegration(ITaskTypeIntegrationService taskTypeService,
            ITaskProcessService taskProcessService, Service.Interfaces.ConnectWise.ITicketService ticketConnectWiseService,
            Service.Interfaces.ConnectWise.IUtilityService utilityService, IWatsonContentService watsonContentService,
            IHubContext<TicketHub, ITicketHub> ticketHub, ITicketService ticketService, IRedisService redisService)
        {
            _taskTypeIntegrationService = taskTypeService;
            _taskProcessService = taskProcessService;
            _ticketConnectWiseService = ticketConnectWiseService;
            _utilityService = utilityService;
            _watsonContentService = watsonContentService;
            _ticketHub = ticketHub;
            _ticketService = ticketService;
            _redisService = redisService;
        }

        public async Task<List<TaskProcessDto>> ConfigureIntegration(IntegrationTypes integrationType,
            int[] companiesToIntegrate, int[] taskTypesToProcess, bool isFirstExecution, int backDaysToStartProcess)
        {
            var taskTypeIntegrations = await _taskTypeIntegrationService.GetAllByCompanyAndTaskTypeAsync(integrationType, companiesToIntegrate, taskTypesToProcess);
            int[] companiesToProcess = taskTypeIntegrations.Select(x => x.CompanyIntegration.CompanyId).ToArray();
            var tasks = new List<TaskProcessDto>();
            foreach (var companyId in companiesToProcess)
            {
                var taskTypeIntegrationsByCompany = taskTypeIntegrations.Where(x => x.CompanyIntegration.CompanyId == companyId).ToList();
                foreach (var taskTypeIntegration in taskTypeIntegrationsByCompany)
                {
                    TaskProcessDto tasksProcesses = await _taskProcessService.CreateOrReturnByTaskTypeIntegrationAsync(taskTypeIntegration, isFirstExecution, backDaysToStartProcess);
                    tasks.Add(tasksProcesses);
                }
            }
            return tasks;
        }

        public async Task ExecuteProcesses(TaskProcessDto task)
        {
            try
            {
                task.Status = TaskStatuses.InProgress;
                await UpdateTaskAsync(task);

                await ProcessTask(task);

                task.Status = TaskStatuses.Finished;
                await UpdateTaskAsync(task);
            }
            catch (Exception ex)
            {
                task.Status = TaskStatuses.Failed;
                await UpdateTaskAsync(task);
            }
        }

        private async Task UpdateTaskAsync(TaskProcessDto task)
        {
            await _taskProcessService.UpdateAsync(task);
        }

        private async Task ProcessTask(TaskProcessDto task)
        {
            const int pageSize = 200;
            var lastpage = 1;

            var taskTypeIntegration = task.TaskTypeIntegration;
            var companyIntegration = taskTypeIntegration.CompanyIntegration;
            var integrationConfig = (ConfigurationDto)companyIntegration.IntegrationConfiguration;

            InitializeServicesClients(integrationConfig.ApiKey, integrationConfig.ClientId, integrationConfig.Site);

            var cwCompanies = await _utilityService.GetCompanies(integrationConfig, companyIntegration.Id);

            var direction = ">";
            var sortOrder = "";
            var sort = $"lastUpdated {sortOrder}";

            var historicalDataCutoff = integrationConfig.HistoricalDataCutoff;

            var maxRuntime = DateTime.UtcNow.AddMinutes(taskTypeIntegration.TimeToProcess);
            var lastUpdatedDate = task.StartDate;

            do
            {
                var query = _utilityService.GetTicketQueryFactory(direction, task.StartDate.Value, historicalDataCutoff, integrationConfig);
                var tickets = await _ticketConnectWiseService.GetTicketsFilteredAsync(query, sort, lastpage, pageSize) ?? new List<ConnectWiseSDKServiceModel.Ticket>();

                if (!tickets.Any() && lastpage == 1)
                {
                    var ticketsCount = _ticketConnectWiseService.GetTicketsCount(query);

                    if (ticketsCount != 0)
                    {
                        continue;
                    }

                    break;
                }

                var filteredTickets = FilterTicketsByCompanies(cwCompanies.Select(x => x.Id.Value).ToList(), tickets);

                if (filteredTickets == null)
                {
                    break;
                }

                if (!filteredTickets.Any() && tickets.Any())
                {
                    lastUpdatedDate = DateTime.Parse(tickets.LastOrDefault()?.Info["lastUpdated"]).ToUniversalTime();
                    task.StartDate = lastUpdatedDate;
                    task.EndDate = task.StartDate.Value.AddMinutes(taskTypeIntegration.TimeToProcess);
                    await UpdateTaskAsync(task);
                    continue;
                }
                                
                Parallel.ForEach(filteredTickets, new ParallelOptions { MaxDegreeOfParallelism = 1 }, async (ticket) =>
                {
                    await ProcessTicket(ticket, integrationConfig, companyIntegration);
                });

                var totalProcessed = filteredTickets.Count;
                lastUpdatedDate = DateTime.Parse(filteredTickets.Last().Info["lastUpdated"]).ToUniversalTime();
                task.TotalProcessed = totalProcessed;
                task.StartDate = lastUpdatedDate;
                task.EndDate = task.StartDate.Value.AddMinutes(taskTypeIntegration.TimeToProcess);
                await UpdateTaskAsync(task);
            } while (maxRuntime > DateTime.UtcNow && lastUpdatedDate > historicalDataCutoff);
        }

        private async Task ProcessTicket(ConnectWiseSDKServiceModel.Ticket ticket, ConfigurationDto integrationConfig, CompanyIntegrationDto companyIntegration)
        {  
            var timeEntriesList = await GetTimeEntries(ticket, integrationConfig);

            //if this company is configured to import notes, get notes and pare down to only necessary data points
            var serviceNotes = await GetServiceNotes(ticket, integrationConfig);

            await AssignInitialDescription(ticket, serviceNotes, integrationConfig);

            var details = new Dictionary<string, DiscoveryModelDetails>();
            var count = 1;

            foreach (var time in timeEntriesList)
            {
                details.Add("Time Entry " + count, new DiscoveryModelDetails { Title = time.MemberIdentifier, Time = time.TimeStart + " - " + time.TimeEnd, Description = time.Notes });
                count++;
            }

            count = 1;
            foreach (var note in serviceNotes)
            {
                details.Add("Note " + count, new DiscoveryModelDetails { Title = note.CreatedBy, Time = note.DateCreated, Description = note.Text });
                count++;
            }
            
            var ticketDetails = new List<TicketDetailDto>();
            foreach (var item in details)
            {
                var ticketDetail = new TicketDetailDto
                {
                    Title = item.Value.Title,
                    Time = item.Value.Time,
                    Description = CryptographicUtility.Encrypt(item.Value.Description)
                };
                ticketDetails.Add(ticketDetail);
            }

            var ticketToCreate = new TicketDto()
            {
                CompanyId = companyIntegration.CompanyId,
                IntegrationType = companyIntegration.IntegrationTypeId,
                ExternalTicketId = ticket.Id.Value,
                //CompanyIntegrationId = companyIntegration.Id,
                InitialDescription = ticket.InitialDescription == null ? string.Empty : CryptographicUtility.Encrypt(ticket.InitialDescription),
                CreatedBy = ticket.Info["enteredBy"],
                CreatedDate = _utilityService.GetTicketDateEntered(ticket),
                ContentTitle = ticket.Summary,
                LastUpdatedDate = DateTime.Parse(ticket.Info["lastUpdated"]).ToUniversalTime(),
                LastUpdatedBy = ticket.Info["updatedBy"],
                Details = ticketDetails
            };

            var existingTicket = await _ticketService.GetByExternalIdAsync(ticketToCreate.ExternalTicketId);

            if (existingTicket == null)
            {
                await _ticketService.CreateTicketAndDetailsAsync(ticketToCreate);
                ticketToCreate.TicketStatus = (int)TicketStatuses.Created;
            }
            else
            {
                ticketToCreate.Id = existingTicket.Id;
                ticketToCreate.TicketId = existingTicket.TicketId;
                await _ticketService.UpdateTicketAndDetailsAsync(ticketToCreate);
                ticketToCreate.TicketStatus = (int)TicketStatuses.Updated;
            }

            var ticketsByUser = await _redisService.Get<List<TicketDto>>(ticketToCreate.LastUpdatedBy);
            if (ticketsByUser == null)
            {
                ticketsByUser = new List<TicketDto>();
            }

            var existTicket = ticketsByUser?.Find(x => x.ExternalTicketId == ticketToCreate.ExternalTicketId);

            if (existTicket != null)
            {
                ticketsByUser.Remove(existTicket);                
            }
            
            ticketsByUser.Add(ticketToCreate);

            await _redisService.Set(ticketToCreate.LastUpdatedBy, ticketsByUser);
            await _ticketHub.Clients.All.SendTickets(ticketToCreate);
        }

        private async Task<List<TimeEntryDto>> GetTimeEntries(ConnectWiseSDKServiceModel.Ticket ticket, ConfigurationDto integrationConfig)
        {
            var excludeDiscussion = integrationConfig.SelectedTicketExclusions.Contains("Discussion");
            var excludeInternal = integrationConfig.SelectedTicketExclusions.Contains("Internal");
            var excludeResolution = integrationConfig.SelectedTicketExclusions.Contains("Resolution");
            var shouldGetTimeEntries = integrationConfig.SelectedTicketDetails.Contains("Time Entries");

            if (!shouldGetTimeEntries)
            { 
                return new List<TimeEntryDto>(); 
            }

            var entries = (await _utilityService.GetTimeEntriesFilteredAsync("ChargeToId = " + ticket.Id, "lastUpdated desc", null, null)) ?? new List<ConnectWiseDotNetSDK.ConnectWise.Client.Time.Model.TimeEntry>();

            return (from entry in entries
                    let excludeEntry =
                        ((excludeDiscussion && entry.AddToDetailDescriptionFlag.Value) ||
                         (excludeInternal && entry.AddToInternalAnalysisFlag.Value) ||
                         (excludeResolution && entry.AddToResolutionFlag.Value))
                    where !excludeEntry
                    select new TimeEntryDto
                    {
                        Notes = entry.Notes,
                        MemberIdentifier = (entry.Member == null) ? string.Empty : entry.Member.Identifier,
                        TimeStart = entry.TimeStart.ToString(),
                        TimeEnd = entry.TimeEnd.ToString()
                    }).ToList();
        }

        private async Task<List<ServiceNoteDto>> GetServiceNotes(ConnectWiseSDKServiceModel.Ticket ticket, ConfigurationDto integrationConfig)
        {
            var excludeDiscussion = integrationConfig.SelectedTicketExclusions.Contains("Discussion");
            var excludeInternal = integrationConfig.SelectedTicketExclusions.Contains("Internal");
            var excludeResolution = integrationConfig.SelectedTicketExclusions.Contains("Resolution");
            var shouldGetServiceNotes = integrationConfig.SelectedTicketDetails.Contains("Notes");

            if (!shouldGetServiceNotes)
            {
                return new List<ServiceNoteDto>();
            }

            var notes = await _utilityService.GetServiceNotesFilteredAsync(ticket.Id, null, "dateCreated asc", null, null) ?? new List<ConnectWiseSDKServiceModel.ServiceNote>();

            return (from note in notes
                    let excludeNote =
                        ((excludeDiscussion && note.DetailDescriptionFlag.Value) ||
                         (excludeInternal && note.InternalAnalysisFlag.Value) ||
                         (excludeResolution && note.ResolutionFlag.Value))
                    where !excludeNote
                    select new ServiceNoteDto
                    {
                        Text = note.Text,
                        CreatedBy = note.CreatedBy,
                        DateCreated = note.DateCreated
                    }).ToList();
        }

        private async Task AssignInitialDescription(ConnectWiseSDKServiceModel.Ticket ticket, ICollection<ServiceNoteDto> serviceNotes, ConfigurationDto integrationConfig)
        {
            //initial description is always the first non-internal note, import it if the company is configured for it
            if (integrationConfig.SelectedTicketDetails.Contains("Initial Description"))
            {
                var initialDescription = await _utilityService.GetServiceNotesFilteredAsync(ticket.Id, "internalAnalysisFlag = false", "dateCreated asc", null, 1);
                if (initialDescription != null)
                { 
                    ticket.InitialDescription = initialDescription.FirstOrDefault()?.Text;
                }

                if (!string.IsNullOrEmpty(ticket.InitialDescription) && ticket.InitialDescription.Equals(serviceNotes.FirstOrDefault()?.Text))
                {
                    serviceNotes.Remove(serviceNotes.First());
                }
            }
        }

        private IList<ConnectWiseSDKServiceModel.Ticket> FilterTicketsByCompanies(ICollection<int> connectWiseCompanies, IEnumerable<ConnectWiseSDKServiceModel.Ticket> tickets)
        {
            return (from item in tickets
                    where item.Company.Id != null && connectWiseCompanies.Contains(item.Company.Id.Value)
                    select item).ToList();
        }

        private void InitializeServicesClients(string apiKey, string clientId, string site)
        {
            _ticketConnectWiseService.InitializeClient(apiKey, clientId, site);
            _utilityService.InitializeClient(apiKey, clientId, site);
        }
    }
}
