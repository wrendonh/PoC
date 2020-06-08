using AutoMapper;
using PersistingPoC.Repository.Interfaces.Sql;
using PersistingPoC.Service.Dtos;
using PersistingPoC.Service.Interfaces.Sql;
using System.Collections.Generic;
using System.Threading.Tasks;
using static PersistingPoC.Entities.Enums;

namespace PersistingPoC.Service.Services.Sql
{
    public class TaskTypeIntegrationService : ITaskTypeIntegrationService
    {
        private readonly ITaskTypeIntegrationRepository _taskTypeIntegrationRepository;
        private readonly IMapper _mapper;

        public TaskTypeIntegrationService(ITaskTypeIntegrationRepository taskTypeIntegrationRepository, IMapper mapper)
        {
            _taskTypeIntegrationRepository = taskTypeIntegrationRepository;
            _mapper = mapper;
        }

        public async Task<List<TaskTypeIntegrationDto>> GetAllByCompanyAndTaskTypeAsync(IntegrationTypes integrationType, int[] companiesToProcess, int[] taskTypes)
        {
            var taskTypeIntegrations = await _taskTypeIntegrationRepository.GetAllByCompanyAndTaskTypeAsync(integrationType, companiesToProcess, taskTypes);
            var result = _mapper.Map<List<TaskTypeIntegrationDto>>(taskTypeIntegrations);
            return result;
        }
    }
}
