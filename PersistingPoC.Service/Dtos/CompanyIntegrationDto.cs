using AutoMapper;
using Crushbank.Common.Utilities;
using Newtonsoft.Json;
using PersistingPoC.Repository.Models.Sql;
using PersistingPoC.Service.Dtos.ConnectWise;
using static PersistingPoC.Entities.Enums;

namespace PersistingPoC.Service.Dtos
{
    public class CompanyIntegrationDto
    {
        public int Id { get; set; }
        public int IntegrationTypeId { get; set; }
        public string IntegrationConfig { get; set; }
        public int CompanyId { get; set; }
        public bool Enabled { get; set; }
                
        public IntegrationTypeDto IntegrationType { get; set; }
        public CompanyDto Company { get; set; }
        public BaseConfigDto IntegrationConfiguration { get; set; }
    }

    public class CompanyIntegrationDtoProfile : Profile
    {
        public CompanyIntegrationDtoProfile()
        {
            CreateMap<CompanyIntegration, CompanyIntegrationDto>()
                .ForMember(dest => dest.IntegrationConfiguration, opt =>
                    opt.MapFrom(s => DecryptConfiguration(s.IntegrationTypeId, s.IntegrationConfig)));
            CreateMap<CompanyIntegrationDto, CompanyIntegration>();
        }

        private BaseConfigDto DecryptConfiguration(int integrationTypeId, string configuration)
        {
            switch ((IntegrationTypes)integrationTypeId)
            {
                case IntegrationTypes.ConnectWise:
                    return JsonConvert.DeserializeObject<ConfigurationDto>(CryptographicUtility.Decrypt(configuration));                
                default:
                    return new BaseConfigDto();
            }
        }
    }
}
