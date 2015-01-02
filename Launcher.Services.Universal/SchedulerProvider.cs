using System.Reactive.Concurrency;
using Luncher.Services;

namespace Launcher.Services.Universal
{
    public class SchedulerProvider : ISchedulerProvider
    {
        public IScheduler Default { get { return Scheduler.Default; } }
    }
}
