using Luncher.Api;
using System.Collections.Generic;
using System.Linq;

namespace Luncher.ViewModels
{
    public class MainPageViewModel : BindableBase
    {
        private readonly IEnumerable<RestaurantType> _notVisitedRestaurants;

        public string Textt { get; set; }
        public MainPageViewModel()
        {
            _notVisitedRestaurants = LuncherApi.ImHungry(LuncherApi.GetRestaurants("1,2,3,4,5,6"));
            Textt = _notVisitedRestaurants.FirstOrDefault().name;
        }
    }
}
