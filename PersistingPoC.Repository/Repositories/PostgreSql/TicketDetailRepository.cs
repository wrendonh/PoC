using Microsoft.EntityFrameworkCore;
using PersistingPoC.Repository.Interfaces.Sql;
using PersistingPoC.Repository.Models.Sql;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersistingPoC.Repository.Repositories.PostgreSql
{
    public class TicketDetailRepository : PostgreSqlRepository<TicketDetail>, ITicketDetailRepository
    {
        private readonly DbContextOptions<PostgreSqlDbContext> _options;

        public TicketDetailRepository(DbContextOptions<PostgreSqlDbContext> options) : base(options)
        {
            _options = options;
        }

        public async Task<List<TicketDetail>> GetAllByTicketIdAsync(int ticketId)
        {
            await using var dbcontext = new PostgreSqlDbContext(_options);
            var query = from ticketDet in dbcontext.TicketDetails
                        where ticketDet.TicketId == ticketId
                        select ticketDet;

            return await query.ToListAsync();
        }
    }
}
