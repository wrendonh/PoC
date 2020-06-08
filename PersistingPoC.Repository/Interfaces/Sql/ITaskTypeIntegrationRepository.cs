using PersistingPoC.Repository.Models.Sql;
using System.Collections.Generic;
using System.Threading.Tasks;
using static PersistingPoC.Entities.Enums;

namespace PersistingPoC.Repository.Interfaces.Sql
{
    public interface ITaskTypeIntegrationRepository : ISqlRepository<TaskTypeIntegration>
    {
        Task<List<TaskTypeIntegration>> GetAllByCompanyAndTaskTypeAsync(IntegrationTypes integrationType, int[] companiesToProcess, int[] taskTypes);
    }
}
