using AutoMapper;
using PersistingPoC.Repository.Models.Sql;

namespace PersistingPoC.Service.Dtos
{
    public class TaskTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class TaskTypeDtoProfile : Profile
    {
        public TaskTypeDtoProfile()
        {
            CreateMap<TaskType, TaskTypeDto>();
            CreateMap<TaskTypeDto, TaskType>();
        }
    }
}
