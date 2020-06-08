using PersistingPoC.Repository.Models.Sql;
using System.Threading.Tasks;
using static PersistingPoC.Entities.Enums;

namespace PersistingPoC.Repository.Interfaces.Sql
{
    public interface ITaskProcessRepository : ISqlRepository<TaskProcess>
    {
        Task<TaskProcess> GetTaskByStatusAndTaskTypeIntegrationAsync(int taskTypeIntegrationId, TaskStatuses[] statuses);
        Task<TaskProcess> GetFullTaskAsync(int taskProcessId);
    }
}
