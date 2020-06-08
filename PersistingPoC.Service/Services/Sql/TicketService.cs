using AutoMapper;
using PersistingPoC.Repository.Interfaces.Sql;
using PersistingPoC.Service.Dtos;
using PersistingPoC.Service.Interfaces;
using PersistingPoC.Service.Interfaces.Sql;
using System.Threading.Tasks;

namespace PersistingPoC.Service.Services.Sql
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ITicketDetailService _ticketDetailService;
        private readonly IMapper _mapper;

        public TicketService(ITicketRepository ticketRepository, ITicketDetailService ticketDetailService,
            IMapper mapper)
        {
            _ticketRepository = ticketRepository;
            _ticketDetailService = ticketDetailService;
            _mapper = mapper;
        }

        public async Task CreateTicketAndDetailsAsync(TicketDto ticketToCreate)
        {
            var ticket = _mapper.Map<Repository.Models.Sql.Ticket>(ticketToCreate);
            await _ticketRepository.InsertAsync(ticket);

            foreach (var ticketDetail in ticketToCreate.Details)
            {
                ticketDetail.TicketId = ticket.Id;
                await _ticketDetailService.CreateAsync(ticketDetail);
            }
        }

        public async Task<TicketDto> GetByExternalIdAsync(int externalTicketId)
        {
            var ticket = await _ticketRepository.GetByExternalIdAsync(externalTicketId);
            if (ticket == null)
            { 
                return null; 
            }
                       
            var result = _mapper.Map<TicketDto>(ticket);
            return result;
        }

        public async Task UpdateTicketAndDetailsAsync(TicketDto ticketToCreate)
        {
            var ticket = _mapper.Map<Repository.Models.Sql.Ticket>(ticketToCreate);
            await _ticketRepository.UpdateAsync(ticket);

            await _ticketDetailService.DeleteByTicketIdAsync(ticketToCreate.Id);

            foreach (var ticketDetail in ticketToCreate.Details)
            {
                ticketDetail.TicketId = ticket.Id;
                await _ticketDetailService.CreateAsync(ticketDetail);
            }
        }
    }
}
