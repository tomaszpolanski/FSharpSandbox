using System.Linq;
using System.Collections.Generic;
using Microsoft.Practices.Prism.StoreApps;
using Luncher.Api;

namespace Luncher.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : VisualStateAwarePage
    {
        public MainPage()
        {
            this.InitializeComponent();
            var list = LuncherApi.GetRestaurants("1,2,3,4,5,6");
            var res = GetNotVisitedRestaurants(list).Take(10).ToList();
            
        }

        private static IEnumerable<Api.RestaurantType> GetNotVisitedRestaurants(IEnumerable<Api.RestaurantType> allRestaurant)
        {
            return LuncherApi.ImHungry(allRestaurant);
        }
    }
}
