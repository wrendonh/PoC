using AutoMapper;
using RepositoryModelsSql = PersistingPoC.Repository.Models.Sql;
using RepositoryModelsMongodb = PersistingPoC.Repository.Models.Mongodb;
using System;
using System.Collections.Generic;

namespace PersistingPoC.Service.Dtos
{
    public class TicketDto
    {
        public int Id { get; set; }
        public int ExternalTicketId { get; set; }
        public int CompanyId { get; set; }
        public int IntegrationType { get; set; }
        public string InitialDescription { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ContentTitle { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        
        public string TicketId { get; set; }
        public int TicketStatus { get; set; }
        
        public List<TicketDetailDto> Details { get; set; }
    }

    public class TicketDtoProfile : Profile
    {
        public TicketDtoProfile()
        {
            CreateMap<RepositoryModelsSql.Ticket, TicketDto>()
                .ForMember(dest => dest.TicketId, opt => opt.Ignore())                
                .ForMember(dest => dest.Details, opt => opt.Ignore())
                .ForMember(dest => dest.TicketStatus, opt => opt.Ignore());
            CreateMap<TicketDto, RepositoryModelsSql.Ticket>();

            CreateMap<RepositoryModelsMongodb.Ticket, TicketDto>()
                .ForMember(dest => dest.TicketId, opt => opt.MapFrom(s => s.Id))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TicketStatus, opt => opt.Ignore());
            CreateMap<TicketDto, RepositoryModelsMongodb.Ticket>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(s => s.TicketId));
        }
    }
}
