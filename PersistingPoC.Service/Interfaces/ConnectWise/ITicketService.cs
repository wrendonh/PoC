using ConnectWiseDotNetSDK.ConnectWise.Client.Service.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PersistingPoC.Service.Interfaces.ConnectWise
{
    public interface ITicketService
    {
        void InitializeClient(string apiKey, string clientId, string site);
        Task<IList<Ticket>> GetTicketsFilteredAsync(string conditions, string orderBy, int? page, int? take);
        int GetTicketsCount(string conditions);
    }
}
