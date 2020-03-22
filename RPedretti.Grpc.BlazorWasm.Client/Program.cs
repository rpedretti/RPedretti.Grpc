using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using RPedretti.Grpc.Client.Shared.Configuration;
using RPedretti.Grpc.Client.Shared.Grpc;
using RPedretti.Grpc.Client.Shared.Interfaces;
using RPedretti.Grpc.Client.Shared.Services;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace RPedretti.Grpc.BlazorWasm.Client
{
    public class Program
    {
        private static void RegisterDependencies(WebAssemblyHostBuilder builder)
        {
            var services = builder.Services;
            var fileProvider = new EmbeddedFileProvider(Assembly.GetExecutingAssembly());
            var configuration = builder.Configuration
                .AddJsonFile(provider: fileProvider, path: "appsettings.json", optional: false, reloadOnChange: false)
                .Build();

            services.AddSingleton<IFileProvider>(fileProvider);
            services.AddSingleton<IGrpcServerConfig>(configuration.GetSection("GrpcServer").Get<AppConfig>());
            services.AddSingleton<IConfiguration>(configuration);

            services.AddSingleton<IMoviesClient>(sp =>
            {
                // Create a gRPC-Web channel pointing to the backend server

                var config = sp.GetService<IGrpcServerConfig>();
                var httpClient = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()));

                var channel = GrpcChannel.ForAddress(config.Url, new GrpcChannelOptions { HttpClient = httpClient });

                // Now we can instantiate gRPC clients for this channel
                return new MovieClientWrapper(new Movies.MoviesClient(channel));
            });

            services.AddSingleton<IMovieService, MovieService>();
            services.AddSingleton<ISecurityService, SecurityService>();
        }

        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.Services.AddBaseAddressHttpClient();

            RegisterDependencies(builder);

            builder.RootComponents.Add<App>("app");

            await builder.Build().RunAsync();
        }
    }
}