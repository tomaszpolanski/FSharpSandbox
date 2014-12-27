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
    public class MainPageViewModel : BindableBase, IDisposable
    {

        public ReadonlyReactiveProperty<string> RestaurantText { get; private set; }
        public ReadonlyReactiveProperty<string> PickedRestaurantText { get; private set; }

        private readonly IDisposable _speechSubscription;

        public MainPageViewModel(IFileSystemService fileSystemService, IGestureRecognizerService gestureService, ITextToSpeechService speechService)
        {
            var restaurantObservable = DefineRestaurants(Observable.FromAsync(token => ReadRestaurantFileAsync("Restaurants.txt", fileSystemService, token)));
            var currentRestaurantOb = DefineRestaurantText(gestureService.SwipeObservable
                                                                         .Where(swipe => swipe == SwipeType.Left)
                                                                         .SelectUnit(), restaurantObservable);
            RestaurantText = currentRestaurantOb
                .Select(TextDescription)
                       .ToReadonlyReactiveProperty(string.Empty);

            PickedRestaurantText = gestureService.SwipeObservable.Select(swipe => swipe == SwipeType.Right)
                .CombineLatest(currentRestaurantOb, (accept, restaurant) => string.IsNullOrEmpty(restaurant) || !accept ? string.Empty : string.Format("Let's go for {0}!", restaurant))
                .ToReadonlyReactiveProperty();

            _speechSubscription = PickedRestaurantText.Where(text => !string.IsNullOrEmpty(text))
                                                      .Select(text => Observable.FromAsync(token => speechService.PlayTextAsync(text, token)))
                                                      .Switch()
                                                      .Subscribe(_speechSubscription => { });
        }

        public void Dispose()
        {
            RestaurantText.Dispose();
            PickedRestaurantText.Dispose();
            _speechSubscription.Dispose();
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
