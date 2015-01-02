using System.Reactive.Concurrency;

namespace Luncher.Services
{
    public interface ISchedulerProvider
    {
        IScheduler Default { get; }
    }
}
