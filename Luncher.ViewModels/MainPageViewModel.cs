using Luncher.Api;
using System.Linq;
using Utilities.Reactive;
using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Luncher.Services;
using System.Reactive;

namespace Luncher.ViewModels
{
    public class MainPageViewModel : BindableBase
    {

        public ReadonlyReactiveProperty<string> RestaurantText { get; private set; }

        public ReactiveCommand NextCommand { get; private set; }

        public MainPageViewModel(IFileSystemService fileSystemService, IGestureRecognizerService gestureService)
        {
            NextCommand = new ReactiveCommand();
            var restaurantObservable = DefineRestaurants(Observable.FromAsync(token => ReadRestaurantFileAsync("Restaurants.txt", fileSystemService, token)));
            RestaurantText = DefineRestaurantText(gestureService.SwipeObservable, restaurantObservable)
                       .ToReadonlyReactiveProperty(string.Empty);
        }

        private static Task<string> ReadRestaurantFileAsync(string fileName, IFileSystemService fileSystemService, CancellationToken token)
        {
            return fileSystemService.ReadEmbeddedFileAsync(fileName, token);
        }

        private static IObservable<RestaurantType> DefineRestaurants(IObservable<string> restaurantOb)
        {
            return
                restaurantOb.Select(restaurants => LuncherApi.GetRestaurants(restaurants)) 
                            .SelectMany( r => LuncherApi.ImHungry(r).Take(100).ToObservable())
                            .Publish()
                            .RefCount();
        }

        private static IObservable<string> DefineRestaurantText(IObservable<Unit> commandTrigger, IObservable<RestaurantType> restaurantObservable)
        {
            return commandTrigger.StartWith(Unit.Default)
                       .Zip(restaurantObservable, (_, restaurant) => restaurant)
                       .Select(restaurant => restaurant.Name)
                       .Select(TextDescription);
        }

        private static string TextDescription(string restaurant)
        {
            return string.IsNullOrEmpty(restaurant) ? "You are sooo picky!\nTry again!"
                                                    : string.Format("How about {0}?", restaurant);
        }

    }
}
