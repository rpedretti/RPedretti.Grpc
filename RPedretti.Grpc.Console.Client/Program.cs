using RPedretti.Grpc.Client.Shared.Factory;
using RPedretti.Grpc.Client.Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPedretti.Grpc.Console.Client
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var client = MovieClientFactory.CreateClient("localhost:444");

            var service = new MovieService(client, null);

            var movies = await service.FindByCriteriaAsync(new Grpc.Client.Shared.Models.SearchCriteria { 
                Title = "s"
            });

            System.Console.WriteLine(string.Join("\n", movies.Select(m => $"{m.Title}, {m.ReleaseDate}")));
            System.Console.ReadKey();
        }
    }
}
