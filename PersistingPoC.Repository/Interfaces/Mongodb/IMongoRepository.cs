namespace PersistingPoC.Repository.Interfaces.Mongodb
{
    public interface IMongoRepository<T> : IRepository<T> where T : class
    {
        T GetById(string id);
    }
}
