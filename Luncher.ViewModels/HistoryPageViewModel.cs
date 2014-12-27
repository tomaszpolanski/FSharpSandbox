using Luncher.Api;
using Luncher.Services;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using System.Reactive.Linq;
using Utilities.Reactive;

namespace Luncher.ViewModels
{
    public class HistoryPageViewModel : BindableBase, IDisposable
    {
        private readonly IDisposable _pickedRestaurantSubscription;

        public ObservableCollection<RestaurantType> HistoryList { get; private set; }

        public ICommand GoBackCommand { get; private set; }


        public HistoryPageViewModel(IHistoryRepository historyRepository,
            INavigator navigator)
        {
            HistoryList = new ObservableCollection<RestaurantType>();
            Debug.WriteLine("grrrrrr");
            _pickedRestaurantSubscription = historyRepository.PickedRestaurantObservable
                .DelaySubscription(TimeSpan.FromSeconds(1))
                .ObserveOnUI()
                .Subscribe(AddHistoryItem);

            GoBackCommand = new DelegateCommand(navigator.GoBack);

            
        }

        public void Dispose()
        {
            _pickedRestaurantSubscription.Dispose();
        }

        private void AddHistoryItem(RestaurantType restaurant)
        {
            Debug.WriteLine(restaurant.Name);
            HistoryList.Add(restaurant);
        }
    }
}