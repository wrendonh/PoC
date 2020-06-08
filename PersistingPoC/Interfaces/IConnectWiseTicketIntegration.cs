using PersistingPoC.Service.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;
using static PersistingPoC.Entities.Enums;

namespace PersistingPoC.Interfaces
{
    public interface IConnectWiseTicketIntegration
    {
        Task<List<TaskProcessDto>> ConfigureIntegration(IntegrationTypes integrationType, int[] companiesToIntegrate, 
            int[] taskTypesToProcess, bool isFirstExecution, int backDaysToStartProcess);
        Task ExecuteProcesses(TaskProcessDto task);
    }
}
