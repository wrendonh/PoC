using AutoMapper;
using PersistingPoC.Repository.Interfaces.Sql;
using PersistingPoC.Repository.Models.Sql;
using PersistingPoC.Service.Dtos;
using PersistingPoC.Service.Interfaces.Sql;
using System.Threading.Tasks;

namespace PersistingPoC.Service.Services.Sql
{
    public class TicketDetailService : ITicketDetailService
    {
        private readonly ITicketDetailRepository _ticketDetailRepository;
        private readonly IMapper _mapper;

        public TicketDetailService(ITicketDetailRepository ticketDetailRepository, IMapper mapper)
        {
            _ticketDetailRepository = ticketDetailRepository;
            _mapper = mapper;
        }

        public async Task CreateAsync(TicketDetailDto ticketDetailToCreate)
        {
            var ticketDetail = _mapper.Map<TicketDetail>(ticketDetailToCreate);
            await _ticketDetailRepository.InsertAsync(ticketDetail);
        }

        public async Task DeleteByTicketIdAsync(int ticketId)
        {
            var ticketDetails = await _ticketDetailRepository.GetAllByTicketIdAsync(ticketId);
            foreach (var ticketDetail in ticketDetails)
            {
                await _ticketDetailRepository.DeleteAsync(ticketDetail);
            }
        }
    }
}
