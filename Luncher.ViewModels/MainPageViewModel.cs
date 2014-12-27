using Luncher.Api;
using System.Linq;
using Utilities.Reactive;
using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Luncher.Services;
using System.Reactive;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;


namespace Luncher.ViewModels
{
    public class MainPageViewModel : BindableBase, IDisposable
    {

        public ReadonlyReactiveProperty<string> RestaurantText { get; private set; }
        public ReadonlyReactiveProperty<string> PickedRestaurantText { get; private set; }

        private readonly IDisposable _speechSubscription;
        private readonly IDisposable _historySubscription;

        public ICommand HistoryCommand { get; private set; }

        public MainPageViewModel(IFileSystemService fileSystemService, 
            IGestureRecognizerService gestureService, 
            ITextToSpeechService speechService,
            IHistoryRepository historyRepository,
            INavigator navigator)
        {
            var restaurantObservable = DefineRestaurants(Observable.FromAsync(token => ReadRestaurantFileAsync("Restaurants.txt", fileSystemService, token)));
            var currentRestaurantOb = DefineRestaurantText(gestureService.SwipeObservable
                                                                         .Where(swipe => swipe == SwipeType.Left)
                                                                         .SelectUnit(), restaurantObservable);
            RestaurantText = currentRestaurantOb
                .Select(TextDescription)
                       .ToReadonlyReactiveProperty(string.Empty);

            var pickedRestaurant = gestureService.SwipeObservable.Select(swipe => swipe == SwipeType.Right)
                .CombineLatest(currentRestaurantOb, (accept, restaurant) => !accept ? Restaurant.Empty : restaurant)
                .Where(restaurant => !Restaurant.IsEmpty(restaurant));
            PickedRestaurantText = pickedRestaurant.Select(restaurant => string.Format("Let's go for {0}!", restaurant.Name))
                                                   .ToReadonlyReactiveProperty();

            _speechSubscription = PickedRestaurantText.Select(text => Observable.FromAsync(token => speechService.PlayTextAsync(text, token)))
                                                      .Switch()
                                                      .Subscribe(_ => { });

            _historySubscription = pickedRestaurant.Subscribe(historyRepository.Add);


            HistoryCommand = new DelegateCommand(() => navigator.Navigate("History", null));
        }

        public void Dispose()
        {
            RestaurantText.Dispose();
            PickedRestaurantText.Dispose();
            _speechSubscription.Dispose();
            _historySubscription.Dispose();
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

        private static IObservable<RestaurantType> DefineRestaurantText(IObservable<Unit> commandTrigger, IObservable<RestaurantType> restaurantObservable)
        {
            return commandTrigger.StartWith(Unit.Default)
                       .Zip(restaurantObservable, (_, restaurant) => restaurant);
                       
        }

        private static string TextDescription(RestaurantType restaurant)
        {
            return string.IsNullOrEmpty(restaurant.Name) ? "You are sooo picky!\nTry again!"
                                                    : string.Format("How about {0}?", restaurant.Name);
        }


    }
}
