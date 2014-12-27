using Launcher.Services.Universal;
using Luncher.Services;
using Microsoft.Practices.Prism.StoreApps;
using Windows.UI.Xaml;
using Microsoft.Practices.Unity;


namespace Luncher.Views
{

    public sealed partial class MainPage : VisualStateAwarePage
    {
        public MainPage()
        {
            InitializeComponent();
            var gestureService = (Application.Current as App).Container.Resolve(typeof(IGestureRecognizerService));
            (gestureService as GestureRecognizerService).SetUiElement(this);
        }
    }
}
