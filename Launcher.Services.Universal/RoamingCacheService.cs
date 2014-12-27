using System;
using Windows.Storage;

namespace Launcher.Services.Universal
{
    public class RoamingCacheService : CacheService
    {
        public RoamingCacheService() :
            base(ApplicationData.Current.TemporaryFolder, TimeSpan.FromDays(30))
        {

        }
    }
}
