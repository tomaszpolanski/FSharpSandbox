using System;
using Luncher.Services;
using Microsoft.Practices.Prism.Mvvm.Interfaces;

namespace Launcher.Services.Universal
{
    public class Navigator : INavigator
    {
        private readonly INavigationService _navigationService;
        public Navigator(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }
        public bool CanGoBack()
        {
            return _navigationService.CanGoBack();
        }

        public void ClearHistory()
        {
            _navigationService.ClearHistory();
        }

        public void GoBack()
        {
            _navigationService.GoBack();
        }

        public bool Navigate(string pageToken, object parameter)
        {
            return _navigationService.Navigate(pageToken, parameter);
        }

        public void RestoreSavedNavigation()
        {
            _navigationService.RestoreSavedNavigation();
        }

        public void Suspending()
        {
            _navigationService.Suspending();
        }
    }
}
