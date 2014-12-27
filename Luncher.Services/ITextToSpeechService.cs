using System.Threading;
using System.Threading.Tasks;

namespace Luncher.Services
{
    public interface ITextToSpeechService
    {
        Task PlayTextAsync(string text, CancellationToken token);
    }
}
