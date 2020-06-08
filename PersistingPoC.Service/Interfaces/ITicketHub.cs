using PersistingPoC.Service.Dtos;
using System.Threading.Tasks;

namespace PersistingPoC.Service.Interfaces
{
    public interface ITicketHub
    {
        Task SendTickets(TicketDto ticket);
    }
}
