using Microsoft.AspNetCore.SignalR;
using PersistingPoC.Service.Dtos;
using PersistingPoC.Service.Interfaces;
using System.Threading.Tasks;

namespace PersistingPoC.Service.Services
{
    public class TicketHub : Hub<ITicketHub>
    {
        public async Task SendTicketToClients(TicketDto ticket)
        {
            await Clients.All.SendTickets(ticket);
        }
    }
}
