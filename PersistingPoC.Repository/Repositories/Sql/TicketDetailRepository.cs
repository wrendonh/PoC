using Microsoft.EntityFrameworkCore;
using PersistingPoC.Repository.Interfaces.Sql;
using PersistingPoC.Repository.Models.Sql;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersistingPoC.Repository.Repositories.Sql
{
    public class TicketDetailRepository : SqlRepository<TicketDetail>, ITicketDetailRepository
    {
        private readonly DbContextOptions _options;

        public TicketDetailRepository(DbContextOptions options) : base(options)
        {
            _options = options;
        }

        public async Task<List<TicketDetail>> GetAllByTicketIdAsync(int ticketId)
        {
            await using var dbcontext = new SqlServerDbContext(_options);
            var query = from ticketDet in dbcontext.TicketDetails
                        where ticketDet.TicketId == ticketId
                        select ticketDet;

            return await query.ToListAsync();
        }
    }
}
