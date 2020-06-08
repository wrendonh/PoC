using AutoMapper;
using PersistingPoC.Repository.Interfaces.Mongodb;
using PersistingPoC.Service.Dtos;
using PersistingPoC.Service.Interfaces;
using System.Threading.Tasks;

namespace PersistingPoC.Service.Services.Mongodb
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IMapper _mapper;

        public TicketService(ITicketRepository ticketRepository, IMapper mapper)
        {
            _ticketRepository = ticketRepository;
            _mapper = mapper;
        }

        public async Task CreateTicketAndDetailsAsync(TicketDto ticketToCreate)
        {
            var ticket = _mapper.Map<Repository.Models.Mongodb.Ticket>(ticketToCreate);
            ticket.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
            await _ticketRepository.InsertAsync(ticket);
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
            var ticket = _mapper.Map<Repository.Models.Mongodb.Ticket>(ticketToCreate);
            await _ticketRepository.UpdateTicketAndDetailsAsync(ticket);
        }
    }
}
