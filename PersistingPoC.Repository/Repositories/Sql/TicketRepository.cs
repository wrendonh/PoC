using Microsoft.EntityFrameworkCore;
using PersistingPoC.Repository.Interfaces.Sql;
using PersistingPoC.Repository.Models.Sql;
using System.Linq;
using System.Threading.Tasks;

namespace PersistingPoC.Repository.Repositories.Sql
{
    public class TicketRepository : SqlRepository<Ticket>, ITicketRepository
    {
        private readonly DbContextOptions _options;

        public TicketRepository(DbContextOptions options) : base(options)
        {
            _options = options;
        }

        public async Task<Ticket> GetByExternalIdAsync(int externalTicketId)
        {
            await using var dbcontext = new SqlServerDbContext(_options);
            var query = from ticket in dbcontext.Tickets
                        where ticket.ExternalTicketId == externalTicketId
                        select ticket;

            return await query.FirstOrDefaultAsync();
        }
    }
}
