using Microsoft.EntityFrameworkCore;
using PersistingPoC.Repository.Interfaces.Sql;
using PersistingPoC.Repository.Models.Sql;
using System.Linq;
using System.Threading.Tasks;

namespace PersistingPoC.Repository.Repositories.PostgreSql
{
    public class TicketRepository : PostgreSqlRepository<Ticket>, ITicketRepository
    {
        private readonly DbContextOptions<PostgreSqlDbContext> _options;

        public TicketRepository(DbContextOptions<PostgreSqlDbContext> options) : base(options)
        {
            _options = options;
        }

        public async Task<Ticket> GetByExternalIdAsync(int externalTicketId)
        {
            await using var dbcontext = new PostgreSqlDbContext(_options);
            var query = from ticket in dbcontext.Tickets
                        where ticket.ExternalTicketId == externalTicketId
                        select ticket;

            return await query.FirstOrDefaultAsync();
        }
    }
}
