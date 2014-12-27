using Launcher.Services.Universal;
using Luncher.Services;
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
        public readonly IUnityContainer Container = new UnityContainer();

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
            Container.RegisterInstance(NavigationService);
            Container.RegisterInstance(SessionStateService);

            Container.RegisterType<IFileSystemService, FileSystemService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IGestureRecognizerService, GestureRecognizerService>(new ContainerControlledLifetimeManager());
            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver(viewType => GetViewModelType(viewType.Name));
            return base.OnInitializeAsync(args);
        }

        private static Type GetViewModelType(string name)
        {
            var viewModelTypeName = string.Format(CultureInfo.InvariantCulture,
                "Luncher.ViewModels.{0}ViewModel, Luncher.ViewModels, Version=1.0.0.0, Culture=neutral",
                name);
            var viewModel = Type.GetType(viewModelTypeName);
            return viewModel;
        }

        protected override object Resolve(Type type)
        {
            return Container.Resolve(type);
        }

        protected override void OnRegisterKnownTypesForSerialization()
        {
            base.OnRegisterKnownTypesForSerialization();
        }
    }
}