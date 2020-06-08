using AutoMapper;
using PersistingPoC.Repository.Models.Sql;
using System;
using static PersistingPoC.Entities.Enums;

namespace PersistingPoC.Service.Dtos
{
    public class TaskProcessDto
    {
        public int Id { get; set; }
        public int TaskTypeIntegrationId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public TaskStatuses Status { get; set; }
        public int TotalProcessed { get; set; }

        public TaskTypeIntegrationDto TaskTypeIntegration { get; set; }
    }

    public class TaskProcessDtoProfile : Profile
    {
        public TaskProcessDtoProfile()
        {
            CreateMap<TaskProcess, TaskProcessDto>();
            CreateMap<TaskProcessDto, TaskProcess>();
        }
    }
}
