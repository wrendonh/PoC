using ConnectWiseDotNetSDK.ConnectWise.Client.Service.Api;
using ConnectWiseDotNetSDK.ConnectWise.Client.Service.Model;
using Crushbank.Common.Entities.RestService;
using Crushbank.Core.Interfaces;
using PersistingPoC.Entities;
using PersistingPoC.Service.Interfaces.ConnectWise;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PersistingPoC.Service.Services.ConnectWise
{
    public class TicketService : BaseService, ITicketService
    {
        private readonly IIntegrationRestService _integrationRestService;        

        public TicketService(IIntegrationRestService integrationRestService)
        {
            _integrationRestService = integrationRestService;
        }

        public async Task<IList<Ticket>> GetTicketsFilteredAsync(string conditions, string orderBy, int? page, int? take)
        {
            var company = new TicketsApi(Client);
            var resp = await company.GetTicketsAsync(conditions, orderBy, page: page, pageSize: take);
            return FormatResponse<IList<Ticket>>(resp);
        }

        public int GetTicketsCount(string conditions)
        {
            var restServiceRequest = new RestServiceRequest(Crushbank.Common.Entities.Enums.IntegrationTypes.ConnectWise)
            {
                Url = Url,
                Resource = $"{Constants.ApiUrlGetTicketsConnectWise}/count"
            };

            restServiceRequest.Headers.Add("Authorization", $"basic {ApiKey}");
            restServiceRequest.Headers.Add("ClientId", ClientId);

            restServiceRequest.Parameters.TryAdd("conditions", conditions);

            var resultResponse = _integrationRestService.Get(restServiceRequest);
            return (int)resultResponse.Content["count"];
        }
    }
}
