using System.Threading;
using System.Threading.Tasks;

namespace Luncher.Services
{
    public interface ICacheService
    {
        Task<T> GetDataAsync<T>(string cacheKey, CancellationToken token);
        Task SaveDataAsync<T>(string cacheKey, T content, CancellationToken token);
    }
}
