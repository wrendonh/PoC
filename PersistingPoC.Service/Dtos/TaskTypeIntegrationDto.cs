using AutoMapper;
using PersistingPoC.Repository.Models.Sql;

namespace PersistingPoC.Service.Dtos
{
    public class TaskTypeIntegrationDto
    {
        public int Id { get; set; }
        public int CompanyIntegrationId { get; set; }
        public int TaskTypeId { get; set; }
        public int TimeToProcess { get; set; } 

        public CompanyIntegrationDto CompanyIntegration { get; set; }
        public TaskTypeDto TaskType { get; set; }
    }

    public class TaskTypeIntegrationDtoProfile : Profile
    {
        public TaskTypeIntegrationDtoProfile()
        {
            CreateMap<TaskTypeIntegration, TaskTypeIntegrationDto>();
            CreateMap<TaskTypeIntegrationDto, TaskTypeIntegration>();
        }
    }
}
