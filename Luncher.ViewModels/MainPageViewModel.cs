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
        public ReadonlyReactiveProperty<string> PickedRestaurantText { get; private set; }

        public ReactiveCommand NextCommand { get; private set; }

        public MainPageViewModel(IFileSystemService fileSystemService, IGestureRecognizerService gestureService)
        {
            NextCommand = new ReactiveCommand();
            var restaurantObservable = DefineRestaurants(Observable.FromAsync(token => ReadRestaurantFileAsync("Restaurants.txt", fileSystemService, token)));
            var currentRestaurantOb = DefineRestaurantText(gestureService.SwipeObservable.Where(swipe => swipe == SwipeType.Left).SelectUnit(), restaurantObservable);
            RestaurantText = currentRestaurantOb
                .Select(TextDescription)
                       .ToReadonlyReactiveProperty(string.Empty);

            PickedRestaurantText = gestureService.SwipeObservable.Where(swipe => swipe == SwipeType.Right)
                .CombineLatest(currentRestaurantOb, (_, restaurant) => string.Format("Let's go for {0}!", restaurant))
                .ToReadonlyReactiveProperty();
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
                       .Select(restaurant => restaurant.Name);
                       
        }

        private static string TextDescription(string restaurant)
        {
            return string.IsNullOrEmpty(restaurant) ? "You are sooo picky!\nTry again!"
                                                    : string.Format("How about {0}?", restaurant);
        }

    }
}
