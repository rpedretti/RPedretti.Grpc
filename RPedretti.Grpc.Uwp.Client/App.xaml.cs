using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Prism.Mvvm;
using Prism.Unity.Windows;
using Prism.Windows.AppModel;
using RPedretti.Grpc.Client.Shared.Configuration;
using RPedretti.Grpc.Client.Shared.Factory;
using RPedretti.Grpc.Client.Shared.Services;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Resources;
using Windows.Security.Cryptography.Certificates;
using Windows.UI.Xaml;

#nullable enable
namespace RPedretti.Grpc.Uwp.Client
{
    [Windows.UI.Xaml.Data.Bindable]
    public sealed partial class App : PrismUnityApplication
    {
        public App()
        {
            InitializeComponent();
        }

        protected override void ConfigureContainer()
        {
            // register a singleton using Container.RegisterType<IInterface, Type>(new ContainerControlledLifetimeManager());
            base.ConfigureContainer();
            Container.RegisterInstance<IResourceLoader>(new ResourceLoaderAdapter(new ResourceLoader()));
            Container.RegisterType<IMovieService, MovieService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ISecurityService, SecurityService>(new ContainerControlledLifetimeManager());
            Container.RegisterInstance<IGrpcServerConfig>(new AppConfig { Url = "https://localhost:4443" });
            RegisterHttpClient();
            RegisterGrpcClient();
        }

        private void RegisterGrpcClient()
        {
            var client = MovieClientFactory.CreateClient("localhost:444");
            Container.RegisterInstance(client);
        }

        private void RegisterHttpClient()
        {
            var httpClient = new HttpClient();
            Container.RegisterInstance(httpClient);
        }

        protected override async Task OnLaunchApplicationAsync(LaunchActivatedEventArgs args)
        {
            await LaunchApplicationAsync(PageTokens.MainPage, null);
        }

        private async Task LaunchApplicationAsync(string page, object? launchParam)
        {
            NavigationService.Navigate(page, launchParam);
            Window.Current.Activate();
            await Task.CompletedTask;
        }

        protected override async Task OnActivateApplicationAsync(IActivatedEventArgs args)
        {
            await Task.CompletedTask;
        }

        protected override async Task OnInitializeAsync(IActivatedEventArgs args)
        {
            await base.OnInitializeAsync(args);

            // We are remapping the default ViewNamePage and ViewNamePageViewModel naming to ViewNamePage and ViewNameViewModel to
            // gain better code reuse with other frameworks and pages within Windows Template Studio
            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) =>
            {
                var viewModelTypeName = string.Format(CultureInfo.InvariantCulture, "RPedretti.Grpc.Uwp.Client.ViewModels.{0}ViewModel, RPedretti.Grpc.Uwp.Client", viewType.Name.Substring(0, viewType.Name.Length - 4));
                return Type.GetType(viewModelTypeName);
            });
        }

        protected override IDeviceGestureService OnCreateDeviceGestureService()
        {
            var service = base.OnCreateDeviceGestureService();
            service.UseTitleBarBackButton = false;
            return service;
        }
    }
}
#nullable restore
