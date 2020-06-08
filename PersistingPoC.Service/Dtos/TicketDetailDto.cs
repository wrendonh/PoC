using AutoMapper;
using RepositoryModelsSql = PersistingPoC.Repository.Models.Sql;
using RepositoryModelsMongodb = PersistingPoC.Repository.Models.Mongodb;

namespace PersistingPoC.Service.Dtos
{
    public class TicketDetailDto
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string Title { get; set; }
        public string Time { get; set; }
        public string Description { get; set; }

        public TicketDto Ticket { get; set; }
    }

    public class TicketDetailDtoProfile : Profile
    {
        public TicketDetailDtoProfile()
        {
            CreateMap<RepositoryModelsSql.TicketDetail, TicketDetailDto>();
            CreateMap<TicketDetailDto, RepositoryModelsSql.TicketDetail>();
            CreateMap<RepositoryModelsMongodb.TicketDetail, TicketDetailDto>()
                 .ForMember(dest => dest.TicketId, opt => opt.Ignore());
            CreateMap<TicketDetailDto, RepositoryModelsMongodb.TicketDetail>();
        }
    }
}
