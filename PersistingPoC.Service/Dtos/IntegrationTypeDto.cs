using AutoMapper;
using PersistingPoC.Repository.Models.Sql;

namespace PersistingPoC.Service.Dtos
{
    public class IntegrationTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class IntegrationTypeDtoProfile : Profile
    {
        public IntegrationTypeDtoProfile()
        {
            CreateMap<IntegrationType, IntegrationTypeDto>();
            CreateMap<IntegrationTypeDto, IntegrationType>();
        }
    }
}
