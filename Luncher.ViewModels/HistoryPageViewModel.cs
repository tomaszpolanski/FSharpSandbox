using Luncher.Api;
using Luncher.Services;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Luncher.ViewModels
{
    public class HistoryPageViewModel : BindableBase, IDisposable
    {
        private readonly IDisposable _sharedItemsSubscription;

        public ObservableCollection<RestaurantType> HistoryList { get; private set; }

        public ICommand GoBackCommand { get; private set; }


        public HistoryPageViewModel(IHistoryRepository historyRepository,
            INavigator navigator)
        {
            HistoryList = new ObservableCollection<RestaurantType>();

            _sharedItemsSubscription = historyRepository.PickedRestaurantObservable.Subscribe(AddHistoryItem);

            GoBackCommand = new DelegateCommand(navigator.GoBack);
            
        }

        public void Dispose()
        {
            _sharedItemsSubscription.Dispose();
        }

        private void AddHistoryItem(RestaurantType restaurant)
        {
            HistoryList.Add(restaurant);
        }
    }
}