using AutoMapper;
using PersistingPoC.Repository.Interfaces.Sql;
using PersistingPoC.Repository.Models.Sql;
using PersistingPoC.Service.Dtos;
using PersistingPoC.Service.Interfaces.Sql;
using System;
using System.Threading.Tasks;
using static PersistingPoC.Entities.Enums;

namespace PersistingPoC.Service.Services.Sql
{
    public class TaskProcessService : ITaskProcessService
    {
        private readonly ITaskProcessRepository _taskProcessRepository;
        private readonly IMapper _mapper;

        public TaskProcessService(ITaskProcessRepository taskProcessRepository, IMapper mapper)
        {
            _taskProcessRepository = taskProcessRepository;
            _mapper = mapper;
        }

        public async Task<TaskProcessDto> CreateOrReturnByTaskTypeIntegrationAsync(TaskTypeIntegrationDto taskTypeIntegration, bool isFirstExecution, int backDaysToStartProcess)
        {
            var statuses = new TaskStatuses[] { TaskStatuses.Pending, TaskStatuses.InProgress };
            var existingTask = await GetTaskByStatusAndTaskTypeIntegrationAsync(taskTypeIntegration.Id, statuses);
            if (existingTask != null)
            {
                if (isFirstExecution)
                {
                    return existingTask;
                }
                else
                {
                    return null;
                }
            }

            statuses = new TaskStatuses[] { TaskStatuses.Finished, TaskStatuses.Failed };
            existingTask = await GetTaskByStatusAndTaskTypeIntegrationAsync(taskTypeIntegration.Id, statuses);
            
            var dateNow = DateTime.UtcNow;
            if (backDaysToStartProcess > 0 && isFirstExecution)
            {
                dateNow = dateNow.AddDays((-1) * backDaysToStartProcess);
                dateNow = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day, 0, 0, 0);
            }

            var taskProcess = new TaskProcess
            {
                TaskTypeIntegrationId = taskTypeIntegration.Id,
                StartDate = existingTask != null ? existingTask.EndDate : dateNow,
                EndDate = existingTask != null ? existingTask.EndDate.Value.AddMinutes(taskTypeIntegration.TimeToProcess) : dateNow.AddMinutes(taskTypeIntegration.TimeToProcess),
                Status = TaskStatuses.Pending
            };

            await _taskProcessRepository.InsertAsync(taskProcess);
            var createdTask = await _taskProcessRepository.GetFullTaskAsync(taskProcess.Id);
            var result = _mapper.Map<TaskProcessDto>(createdTask);
            return result;
        }

        public async Task<TaskProcessDto> GetTaskByStatusAndTaskTypeIntegrationAsync(int taskTypeIntegrationId, TaskStatuses[] statuses)
        {
            var tasks = await _taskProcessRepository.GetTaskByStatusAndTaskTypeIntegrationAsync(taskTypeIntegrationId, statuses);
            var result = _mapper.Map<TaskProcessDto>(tasks);
            return result;
        }

        public async Task UpdateAsync(TaskProcessDto task)
        {
            var taskToUpdate = await _taskProcessRepository.GetByIdAsync(task.Id);
            taskToUpdate.Status = task.Status;
            taskToUpdate.StartDate = task.StartDate;
            taskToUpdate.TaskTypeIntegrationId = task.TaskTypeIntegrationId;
            taskToUpdate.EndDate = task.EndDate;
            await _taskProcessRepository.UpdateAsync(taskToUpdate);
        }
    }
}
