namespace Luncher.Services
{
    public interface INavigator
    {
        bool CanGoBack();
        void ClearHistory();
        void GoBack();
        bool Navigate(string pageToken, object parameter);
        void RestoreSavedNavigation();
        void Suspending();
    }
}
