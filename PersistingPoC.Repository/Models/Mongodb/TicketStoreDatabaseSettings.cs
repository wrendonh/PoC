using PersistingPoC.Repository.Interfaces.Mongodb;

namespace PersistingPoC.Repository.Models.Mongodb
{
    public class TicketStoreDatabaseSettings : ITicketStoreDatabaseSettings
    {
        public string TicketsCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
