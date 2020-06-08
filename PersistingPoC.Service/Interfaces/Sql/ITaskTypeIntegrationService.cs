using PersistingPoC.Service.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;
using static PersistingPoC.Entities.Enums;

namespace PersistingPoC.Service.Interfaces.Sql
{
    public interface ITaskTypeIntegrationService
    {
        Task<List<TaskTypeIntegrationDto>> GetAllByCompanyAndTaskTypeAsync(IntegrationTypes integrationType, int[] companiesToProcess, int[] taskTypes);
    }
}
