using Microsoft.Practices.Prism.StoreApps;
using System;

namespace Luncher.Controls
{
    public class DisposingPage : VisualStateAwarePage
    {
        protected override void OnNavigatedFrom(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            IDisposable viewModel = DataContext as IDisposable;
            if (viewModel != null)
            {
                viewModel.Dispose();
            }
        }
    }
}
