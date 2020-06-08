using ConnectWiseDotNetSDK.ConnectWise.Client.Company.Api;
using ConnectWiseDotNetSDK.ConnectWise.Client.Company.Model;
using ConnectWiseDotNetSDK.ConnectWise.Client.Service.Api;
using ConnectWiseDotNetSDK.ConnectWise.Client.Service.Model;
using ConnectWiseDotNetSDK.ConnectWise.Client.Time.Api;
using ConnectWiseDotNetSDK.ConnectWise.Client.Time.Model;
using Newtonsoft.Json;
using PersistingPoC.Service.Dtos.ConnectWise;
using PersistingPoC.Service.Interfaces.ConnectWise;
using PersistingPoC.Service.Services.ConnectWise;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistingPoC.Service.Services.ConnectWise
{
    public class UtilityService : BaseService, IUtilityService
    {
        private const int CompaniesReloadTime = 1440;
        public static Dictionary<int, List<Company>> _companies;
        private DateTime _dateToRefreshData;

        public UtilityService()
        {   
        }

        public async Task<List<Company>> GetCompanies(ConfigurationDto cwConfig, int integrationId)
        {
            if (_companies == null || DateTime.UtcNow > _dateToRefreshData)
            {
                await InitCompanyDictionary(cwConfig, integrationId);
            }

            _companies.TryGetValue(integrationId, out var companyValues);

            if (companyValues != null && companyValues.Any())
            {
                return companyValues.ConvertAll(c => c);
            }

            if (_companies != null)
            {
                var companyList = await GetCompanyList(cwConfig);
                lock (_companies)
                {
                    _companies.Remove(integrationId);
                    _companies.Add(integrationId, companyList);
                    _companies.TryGetValue(integrationId, out companyValues);
                }
            }
            else
            {
                await InitCompanyDictionary(cwConfig, integrationId);
            }

            return companyValues?.ConvertAll(c => c);
        }

        public Dictionary<string, string> GetProperties(Ticket ticket, ConfigurationDto connectWiseConfigDto, Dictionary<string, string> properties)
        {
            switch (connectWiseConfigDto.VersionNumber)
            {
                case "v2019.4":
                case "v2019.5":
                case "v2020.1":
                    if (ticket.Info.ContainsKey("dateEntered"))
                    {
                        properties.Add("Created Date", ticket.Info["dateEntered"]);
                    }

                    if (ticket.Info.ContainsKey("enteredBy"))
                    {
                        properties.Add("Created by", ticket.Info["enteredBy"]);
                    }

                    if (ticket.Info.ContainsKey("lastUpdated"))
                    {
                        properties.Add("Updated Date", ticket.Info["lastUpdated"]);
                    }

                    if (ticket.Info.ContainsKey("updatedBy"))
                    {
                        properties.Add("Updated by", ticket.Info["updatedBy"]);
                    }

                    return properties;

                default:
                    return properties;
            }
        }

        public DateTime GetTicketDateEntered(Ticket ticket)
        {
            DateTime.TryParse(ticket.Info["dateEntered"], out var result);

            return result.ToUniversalTime();
        }

        public string GetTicketQueryFactory(string direction, DateTime IndexingStartDate, DateTime HistoricalDataCutoff, ConfigurationDto CWConfig)
        {
            var boardIds = AddItemsFromList(CWConfig.SelectedServiceBoards.ConvertAll(x => x.ToString()));

            return CWConfig.VersionNumber switch
            {
                var version when version == "v2019.3" ||
                                 version == "v2019.4" ||
                                 version == "v2019.5" ||
                                 version == "v2020.1"
                => $"lastUpdated {direction} [{IndexingStartDate:yyyy-MM-ddTHH:mm:ssZ}] AND lastUpdated >= [{HistoricalDataCutoff:yyyy-MM-ddTHH:mm:ssZ}] AND board/id in ({boardIds})",
                _ =>
                $"lastUpdated {direction} [{IndexingStartDate:yyyy-MM-dd HH:mm:ss tt}] AND lastUpdated >= [{HistoricalDataCutoff:yyyy-MM-dd HH:mm:ss tt}] AND board/id in ({boardIds})",
            };
        }

        public async Task<IList<TimeEntry>> GetTimeEntriesFilteredAsync(string conditions, string orderBy, int? page, int? take)
        {
            var company = new TimeEntriesApi(Client);
            var resp = await company.GetEntriesAsync(conditions, orderBy, page: page, pageSize: take);
            return FormatResponse<IList<TimeEntry>>(resp);
        }

        public async Task<IList<ServiceNote>> GetServiceNotesFilteredAsync(int? id, string conditions, string orderBy, int? page, int? take)
        {
            var ticket = new TicketNotesApi(Client);
            var resp = await ticket.GetNotesAsync(id, conditions, orderBy, page: page, pageSize: take);
            return FormatResponse<IList<ServiceNote>>(resp);
        }

        private StringBuilder AddItemsFromList(IEnumerable<string> list)
        {
            var query = new StringBuilder("");
            foreach (var item in list)
            {
                if (query.ToString() != "")
                {
                    query.Append(",");
                }

                query.AppendFormat("{0}", item);

            }

            return query;
        }

        private async System.Threading.Tasks.Task InitCompanyDictionary(ConfigurationDto cwConfig,
            int integrationId)
        {
            var reloadValue = CompaniesReloadTime;
            _companies = new Dictionary<int, List<Company>>
            {
                {integrationId, await GetCompanyList(cwConfig)}
            };
            _dateToRefreshData = DateTime.UtcNow.AddMinutes(reloadValue == 0 ? 600 : reloadValue);
        }

        private async Task<List<Company>> GetCompanyList(ConfigurationDto CWConfig)
        {
            var companyPage = 1;
            const int pageSize = 1000;
            var companyListFilteredByStatus = new List<Company>();
            var companyListFilteredByType = new List<CompanyNote>();
            IList<Company> companiesFilteredByStatus;
            IList<CompanyNote> companiesFilteredByType;

            var selectedCompanyStatuses = CWConfig.SelectedCompanyStatuses;
            var selectedCompanyTypes = CWConfig.SelectedCompanyTypes;

            do
            {
                var query = AddQueryParams(selectedCompanyStatuses, "status/name");

                companiesFilteredByStatus =
                    await GetAllCompaniesFilteredAsync(query, "name asc", companyPage, pageSize);

                if (companiesFilteredByStatus == null)
                {
                    break;
                }

                companyListFilteredByStatus.AddRange(companiesFilteredByStatus);
                companyPage++;
            } while (companiesFilteredByStatus.Count == pageSize);

            companyPage = 1;

            do
            {
                var query = AddQueryParams(selectedCompanyTypes, "type/id");

                var companyByTypeIdAsync =
                    await GetCompanyByTypeIdAsync(query, pageSize, companyPage);

                companiesFilteredByType = JsonConvert.DeserializeObject<List<CompanyNote>>(companyByTypeIdAsync);

                if (companiesFilteredByType == null)
                {
                    break;
                }

                companyListFilteredByType.AddRange(companiesFilteredByType);
                companyPage++;
            } while (companiesFilteredByType.Count == pageSize);

            var result = from companyListByStatus in companyListFilteredByStatus
                         join companyType in companyListFilteredByType on companyListByStatus.Id equals companyType.Company.Id
                         select companyListByStatus;

            return result.Distinct().ToList();
        }

        private string AddQueryParams(IReadOnlyCollection<string> items, string attribute, string splitSeparator = ",")
        {
            if (items.Count == 0)
            {
                return string.Empty;
            }

            var newQuery = $"{attribute} in (";
            if (splitSeparator != ",")
            {
                newQuery = "(";
                newQuery = string.Concat(newQuery,
                    string.Join($" OR ",
                        items.Select(c =>
                            $"{attribute} = '{c}' OR {attribute} {splitSeparator} '*,{c},*' OR {attribute} {splitSeparator} '{c},*' OR {attribute} {splitSeparator} '*,{c}'")));
                newQuery = string.Concat(newQuery, ")");
            }
            else
            {
                newQuery = string.Concat(newQuery, string.Join(splitSeparator, items) + ")");
            }

            return newQuery;
        }

        private async Task<IList<Company>> GetAllCompaniesFilteredAsync(string conditions, string orderBy, int page, int take)
        {
            var company = new CompaniesApi(Client);
            var resp = await company.GetCompaniesAsync(conditions, orderBy, page: page, pageSize: take);
            return FormatResponse<IList<Company>>(resp);
        }

        private async Task<string> GetCompanyByTypeIdAsync(string query, int pageSize, int pageNumber)
        {
            var restClient = new RestClient(Url);
            var request = new RestRequest(Method.GET)
            {
                Resource = string.Format($"v4_6_release/apis/3.0/company/companyTypeAssociations/?conditions=({query})")
            };

            request.AddHeader("Authorization", $"Basic {ApiKey}");
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("ClientId", ClientId);
            request.AddParameter("pageSize", pageSize);
            request.AddParameter("page", (pageNumber - 1));

            var response = await restClient.ExecuteAsync(request);

            return response.Content;
        }
    }
}
