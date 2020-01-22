using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RPedretti.Grpc.BlazorWasm.Client.Configuration;
using RPedretti.Grpc.Client.Shared.Configuration;
using RPedretti.Grpc.Client.Shared.Services;
using System.Net.Http;

namespace RPedretti.Grpc.BlazorWasm.Client
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; private set; }

        public void ConfigureServices(IServiceCollection services)
        {
            var configurationBuilder = services
                .AddEmbeddedJson("Configuration/appsettings.json", true)
                .GetJsonConfigurationBuilderBuilder();
            
            Configuration = configurationBuilder.Build();
            var appConfig = new AppConfig();
            Configuration.Bind("GrpcServer", appConfig);


            services.AddSingleton<IConfiguration>(Configuration);
            services.AddSingleton<IGrpcServerConfig>(appConfig);
            
            services.AddSingleton(sp =>
            {
                // Create a gRPC-Web channel pointing to the backend server

                var config = sp.GetService<IGrpcServerConfig>();
                var httpClient = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()));

                var channel = GrpcChannel.ForAddress(config.Url, new GrpcChannelOptions { HttpClient = httpClient });

                // Now we can instantiate gRPC clients for this channel
                return new Movies.MoviesClient(channel);
            });

            services.AddSingleton<IMovieService, MovieService>();
            services.AddSingleton<ISecurityService, SecurityService>();
        
            
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
