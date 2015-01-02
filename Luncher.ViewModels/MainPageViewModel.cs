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
using Microsoft.Practices.Prism.Mvvm;

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
            RestaurantText = currentRestaurantOb.Select(GetTextDescription)
                                                .ToReadonlyReactiveProperty(string.Empty);

            var pickedRestaurant = DefinePickedRestaurant(gestureService.SwipeObservable, currentRestaurantOb);
            PickedRestaurantText = pickedRestaurant.Select(GetPickedRestaurantText)
                                                   .ToReadonlyReactiveProperty();

            _speechSubscription = DefineSpeech(PickedRestaurantText, speechService).Subscribe();

            _historySubscription = DefineHistory(pickedRestaurant).Subscribe(historyRepository.Add);


            HistoryCommand = new DelegateCommand(() => navigator.Navigate("History", null));
        }

        public void Dispose()
        {
            RestaurantText.Dispose();
            PickedRestaurantText.Dispose();
            _speechSubscription.Dispose();
            _historySubscription.Dispose();
        }

        private static IObservable<RestaurantType> DefinePickedRestaurant(IObservable<SwipeType> swipeOb, IObservable<RestaurantType> currentRestaurantOb)
        {
            return swipeOb.Select(swipe => swipe == SwipeType.Right)
                          .CombineLatest(currentRestaurantOb, (accept, restaurant) => !accept ? Restaurant.Empty : restaurant);
        }

        private static Task<string> ReadRestaurantFileAsync(string fileName, IFileSystemService fileSystemService, CancellationToken token)
        {
            return fileSystemService.ReadEmbeddedFileAsync(fileName, token);
        }

        private static IObservable<RestaurantType> DefineRestaurants(IObservable<string> restaurantOb)
        {
            return
                restaurantOb.Select(LuncherApi.GetRestaurants) 
                            .SelectMany( r => LuncherApi.ImHungry(r).Take(100).ToObservable())
                            .Publish()
                            .RefCount();
        }

        private static IObservable<RestaurantType> DefineRestaurantText(IObservable<Unit> commandTrigger, IObservable<RestaurantType> restaurantObservable)
        {
            return commandTrigger.StartWith(Unit.Default)
                                 .Zip(restaurantObservable, (_, restaurant) => restaurant);
        }

        private static IObservable<Unit> DefineSpeech(IObservable<string> textOb, ITextToSpeechService speechService)
        {
            return textOb.Where(text => !string.IsNullOrEmpty(text))
                         .Select(text => Observable.FromAsync(token => speechService.PlayTextAsync(text, token)))
                         .Switch();
        }

        private static IObservable<PickedRestaurantType> DefineHistory(IObservable<RestaurantType> pickedRestaurantOb)
        {
            return pickedRestaurantOb.Where(restaurant => !Restaurant.IsEmpty(restaurant))
                                     .Select(Restaurant.CreatePicked);
        }

        private static string GetTextDescription(RestaurantType restaurant)
        {
            return string.IsNullOrEmpty(restaurant.Name) ? "You are sooo picky!\nTry again!"
                                                         : string.Format("How about {0}?", restaurant.Name);
        }

        private static string GetPickedRestaurantText(RestaurantType restaurant)
        {
            return !Restaurant.IsEmpty(restaurant) ? string.Format("Let's go for {0}!", restaurant.Name) : string.Empty;
        }
    }
}
