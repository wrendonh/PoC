using PersistingPoC.Repository.Models.Sql;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PersistingPoC.Repository.Interfaces.Sql
{
    public interface ITicketDetailRepository : ISqlRepository<TicketDetail>
    {
        Task<List<TicketDetail>> GetAllByTicketIdAsync(int ticketId);
    }
}
