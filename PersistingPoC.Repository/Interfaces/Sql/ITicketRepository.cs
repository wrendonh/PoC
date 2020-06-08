using PersistingPoC.Repository.Models.Sql;
using System.Threading.Tasks;

namespace PersistingPoC.Repository.Interfaces.Sql
{
    public interface ITicketRepository : ISqlRepository<Ticket>
    {
        Task<Ticket> GetByExternalIdAsync(int externalTicketId);
    }
}
