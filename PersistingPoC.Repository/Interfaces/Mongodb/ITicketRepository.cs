using PersistingPoC.Repository.Models.Mongodb;
using System.Threading.Tasks;

namespace PersistingPoC.Repository.Interfaces.Mongodb
{
    public interface ITicketRepository : IMongoRepository<Ticket>
    {
        Task<Ticket> GetByExternalIdAsync(int externalId);
        Task UpdateTicketAndDetailsAsync(Ticket ticket);
    }
}
