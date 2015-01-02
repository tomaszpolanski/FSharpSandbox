using System.Collections.Generic;
using Luncher.Api;
using Luncher.Services;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Reactive.Linq;
using Utilities.Reactive;
using Microsoft.Practices.Prism.Mvvm;

namespace Luncher.ViewModels
{
    public class HistoryPageViewModel : BindableBase, IDisposable
    {
        private readonly IDisposable _pickedRestaurantSubscription;

        public ObservableCollection<PickedRestaurantType> HistoryList { get; private set; }

        public ICommand GoBackCommand { get; private set; }

        public HistoryPageViewModel(IHistoryRepository historyRepository,
            INavigator navigator,
            ISchedulerProvider schedulerProvider)
        {
            HistoryList = new ObservableCollection<PickedRestaurantType>();

            _pickedRestaurantSubscription = historyRepository.PickedRestaurantObservable
                .Buffer(TimeSpan.FromMilliseconds(300), schedulerProvider.Default)
                .DelaySubscription(TimeSpan.FromSeconds(1), schedulerProvider.Default)
                .ObserveOnUI()
                .Subscribe(AddHistoryItem);

            GoBackCommand = new DelegateCommand(navigator.GoBack);
        }

        public void Dispose()
        {
            _pickedRestaurantSubscription.Dispose();
        }

        private void AddHistoryItem(IList<PickedRestaurantType> items)
        {
            foreach (var item in items)
            {
                HistoryList.Insert(0, item);
            }
        }
    }
}