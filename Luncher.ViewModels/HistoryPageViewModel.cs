using Luncher.Api;
using Luncher.Services;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Reactive.Linq;
using Utilities.Reactive;

namespace Luncher.ViewModels
{
    public class HistoryPageViewModel : BindableBase, IDisposable
    {
        private readonly IDisposable _pickedRestaurantSubscription;

        public ObservableCollection<PickedRestaurantType> HistoryList { get; private set; }

        public ICommand GoBackCommand { get; private set; }

        public HistoryPageViewModel(IHistoryRepository historyRepository,
            INavigator navigator)
        {
            HistoryList = new ObservableCollection<PickedRestaurantType>();

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

        private void AddHistoryItem(PickedRestaurantType item)
        {
            HistoryList.Insert(0, item);
        }
    }
}