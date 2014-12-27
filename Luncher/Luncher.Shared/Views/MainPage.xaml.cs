using Launcher.Services.Universal;
using Luncher.Services;
using Windows.UI.Xaml;
using Microsoft.Practices.Unity;
using Luncher.Controls;

namespace Luncher.Views
{

    public sealed partial class MainPage : DisposingPage
    {
        public MainPage()
        {
            InitializeComponent();
            var gestureService = (Application.Current as App).Container.Resolve(typeof(IGestureRecognizerService));
            (gestureService as GestureRecognizerService).SetUiElement(this);
        }
    }
}
