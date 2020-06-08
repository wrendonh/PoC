using PersistingPoC.Service.Dtos;
using System.Threading.Tasks;

namespace PersistingPoC.Service.Interfaces.Sql
{
    public interface ITicketDetailService
    {
        Task CreateAsync(TicketDetailDto ticketDetailToCreate);
        Task DeleteByTicketIdAsync(int ticketId);
    }
}
