using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RPedretti.Grpc.DAL.Accessor;
using RPedretti.Grpc.DAL.Context;

namespace RPedretti.Grpc.DAL
{
    public static class DbInitializerExtension
    {
        private static readonly ILoggerFactory MyLoggerFactory = LoggerFactory.Create(builder => 
        {
            builder.AddConsole(); 
        });

        public static IServiceCollection RegisterSqLite(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Movies");
            services.AddDbContext<MoviesContext>(options =>
                options
                    .UseLoggerFactory(MyLoggerFactory)
                    .UseSqlite(connectionString));

            services.AddScoped<IMoviesAccessor, MoviesAccessor>();

            return services;
        }
    }
}
