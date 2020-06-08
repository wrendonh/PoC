using Microsoft.EntityFrameworkCore;
using PersistingPoC.Repository.Interfaces.Sql;
using PersistingPoC.Repository.Models.Sql;
using System.Linq;
using System.Threading.Tasks;
using static PersistingPoC.Entities.Enums;

namespace PersistingPoC.Repository.Repositories.Sql
{
    public class TaskProcessRepository : SqlRepository<TaskProcess>, ITaskProcessRepository
    {
        private readonly DbContextOptions _options;

        public TaskProcessRepository(DbContextOptions options) : base(options)
        {
            _options = options;
        }

        public async Task<TaskProcess> GetTaskByStatusAndTaskTypeIntegrationAsync(int taskTypeIntegrationId, TaskStatuses[] statuses)
        {
            await using var dbContext = new SqlServerDbContext(_options);
            var query = from tsk in dbContext.TaskProcesses
                        where tsk.TaskTypeIntegrationId == taskTypeIntegrationId && statuses.Contains(tsk.Status)
                        select tsk;

            query = query.Include(x => x.TaskTypeIntegration)
                .Include(x => x.TaskTypeIntegration.CompanyIntegration)
                .OrderByDescending(x => x.StartDate);
            return await query.FirstOrDefaultAsync();
        }

        public async Task<TaskProcess> GetFullTaskAsync(int taskProcessId)
        {
            await using var dbContext = new SqlServerDbContext(_options);
            var query = from tsk in dbContext.TaskProcesses
                        where tsk.Id == taskProcessId
                        select tsk;

            query = query.Include(x => x.TaskTypeIntegration)
                .Include(x => x.TaskTypeIntegration.CompanyIntegration)
                .OrderByDescending(x => x.StartDate);
            return await query.FirstOrDefaultAsync();
        }
    }
}
