using Luncher.Api;
using System.Collections.Generic;
using System.Linq;
using Utilities.Reactive;
using System;
using System.Reactive.Linq;

namespace Luncher.ViewModels
{
    public class MainPageViewModel : BindableBase
    {

        public ReadonlyReactiveProperty<string> RestaurantText { get; private set; }

        public ReactiveCommand NextCommand { get; private set; }

        public MainPageViewModel()
        {
            NextCommand = new ReactiveCommand();
            var restaurantObservable = DefineRestaurants("1,2,3,4,5,6");
            RestaurantText = DefineRestaurantText(NextCommand, restaurantObservable)
                       .ToReadonlyReactiveProperty(string.Empty);
        }

        private static IObservable<RestaurantType> DefineRestaurants(string restaurantList)
        {
            return LuncherApi.ImHungry(LuncherApi.GetRestaurants(restaurantList)).Take(100)
                .ToObservable()
                .Publish()
                .RefCount();
        }

        private static IObservable<string> DefineRestaurantText(IObservable<object> commandTrigger, IObservable<RestaurantType> restaurantObservable)
        {
            return commandTrigger.StartWith((object)null)
                       .Zip(restaurantObservable, (_, restaurant) => restaurant)
                       .Select(restaurant => restaurant.name)
                       .Select(TextDescription);
        }

        private static string TextDescription(string restaurant)
        {
            return string.IsNullOrEmpty(restaurant) ? "You are sooo picky!\nTry again!"
                                                    : string.Format("How about {0}?", restaurant);
        }

    }
}
