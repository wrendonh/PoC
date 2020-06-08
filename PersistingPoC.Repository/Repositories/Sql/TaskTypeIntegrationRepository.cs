using Microsoft.EntityFrameworkCore;
using PersistingPoC.Repository.Interfaces.Sql;
using PersistingPoC.Repository.Models.Sql;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static PersistingPoC.Entities.Enums;

namespace PersistingPoC.Repository.Repositories.Sql
{
    public class TaskTypeIntegrationRepository : SqlRepository<TaskTypeIntegration>, ITaskTypeIntegrationRepository
    {
        private readonly DbContextOptions _options;

        public TaskTypeIntegrationRepository(DbContextOptions options) : base(options)
        {
            _options = options;
        }

        public async Task<List<TaskTypeIntegration>> GetAllByCompanyAndTaskTypeAsync(IntegrationTypes integrationType, int[] companiesToProcess, int[] taskTypes)
        {
            await using var dbContext = new SqlServerDbContext(_options);
            var query = from taskTypeInteg in dbContext.TaskTypeIntegrations
                    join compInteg in dbContext.CompanyIntegrations on taskTypeInteg.CompanyIntegrationId equals compInteg.Id
                    where compInteg.IntegrationTypeId == (int)integrationType
                    select taskTypeInteg;

            if (companiesToProcess.Any() || taskTypes.Any())
            {                
                if (companiesToProcess.Any() && !taskTypes.Any())
                {
                    query = query.Where(x => companiesToProcess.Contains(x.CompanyIntegration.CompanyId));
                }
                else if (!companiesToProcess.Any() && taskTypes.Any())
                {
                    query = query.Where(x => taskTypes.Contains(x.TaskTypeId));
                }
                else
                {
                    query = query.Where(x => companiesToProcess.Contains(x.CompanyIntegration.CompanyId) && taskTypes.Contains(x.TaskTypeId));
                }
            }

            query = query.Include(x => x.CompanyIntegration);
            return await query.ToListAsync();
        }
    }
}
