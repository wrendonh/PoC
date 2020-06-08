namespace PersistingPoC.Repository.Interfaces.Mongodb
{
    public interface ITicketStoreDatabaseSettings
    {
        string TicketsCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
