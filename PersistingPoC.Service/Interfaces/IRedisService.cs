using System.Threading.Tasks;

namespace PersistingPoC.Service.Interfaces
{
    public interface IRedisService
    {
        Task Set<T>(string key, T value);
        Task<T> Get<T>(string key);
    }
}
