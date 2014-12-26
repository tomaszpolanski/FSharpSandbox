using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Unity;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;


namespace Luncher
{
    public sealed partial class App : MvvmAppBase
    {
        private readonly IUnityContainer _container = new UnityContainer();

        public App()
        {
            InitializeComponent();
        }

        protected override Task OnLaunchApplicationAsync(LaunchActivatedEventArgs args)
        {
            NavigationService.Navigate("Main", null);
            Window.Current.Activate();
            return Task.FromResult<object>(null);
        }

        protected override Task OnInitializeAsync(IActivatedEventArgs args)
        {
            _container.RegisterInstance(NavigationService);
            _container.RegisterInstance(SessionStateService);

            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver(viewType =>
            {
                var viewModelTypeName = string.Format(CultureInfo.InvariantCulture,
                    "Luncher.ViewModels.{0}ViewModel, Luncher.ViewModels, Version=1.0.0.0, Culture=neutral",
                    viewType.Name);
                var viewModelType = Type.GetType(viewModelTypeName);

                return viewModelType;
            });
            return base.OnInitializeAsync(args);
        }

        protected override object Resolve(Type type)
        {
            return _container.Resolve(type);
        }

        protected override void OnRegisterKnownTypesForSerialization()
        {
            base.OnRegisterKnownTypesForSerialization();
        }
    }
}