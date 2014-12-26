using Launcher.Services.Universal;
using Luncher.Services;
using Microsoft.Practices.Prism.StoreApps;
using System;
using Windows.UI.Xaml;
using Microsoft.Practices.Unity;


namespace Luncher.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : VisualStateAwarePage
    {
        public MainPage()
        {
            InitializeComponent();
            var test = (Application.Current as App).Container.Resolve(typeof(IGestureRecognizerService));
            (test as GestureRecognizerService).SetUiElement(this);
        }

    }
}
