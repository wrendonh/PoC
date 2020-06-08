using PersistingPoC.Service.Dtos;
using System.Threading.Tasks;
using static PersistingPoC.Entities.Enums;

namespace PersistingPoC.Service.Interfaces.Sql
{
    public interface ITaskProcessService
    {
        Task<TaskProcessDto> GetTaskByStatusAndTaskTypeIntegrationAsync(int taskTypeIntegrationId, TaskStatuses[] statuses);
        Task<TaskProcessDto> CreateOrReturnByTaskTypeIntegrationAsync(TaskTypeIntegrationDto taskTypeIntegration, bool isFirstExecution, int backDaysToStartProcess);
        Task UpdateAsync(TaskProcessDto task);
    }
}
