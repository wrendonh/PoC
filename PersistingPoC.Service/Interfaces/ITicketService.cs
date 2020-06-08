using PersistingPoC.Service.Dtos;
using System.Threading.Tasks;

namespace PersistingPoC.Service.Interfaces
{
    public interface ITicketService
    {
        Task CreateTicketAndDetailsAsync(TicketDto ticketToCreate);
        Task<TicketDto> GetByExternalIdAsync(int externalTicketId);
        Task UpdateTicketAndDetailsAsync(TicketDto ticketToCreate);
    }
}
