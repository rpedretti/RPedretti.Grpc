using Google.Protobuf.WellKnownTypes;
using RJPSoft.HelperExtensions;
using RPedretti.Grpc.Client.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPedretti.Grpc.Client.Shared.Services
{
    public class MovieService : IMovieService
    {
        private readonly Movies.MoviesClient _moviesClient;

        public MovieService(Movies.MoviesClient moviesClient)
        {
            _moviesClient = moviesClient;
        }

        public async Task<IEnumerable<MovieModel>> FindByCriteria(Models.SearchCriteria criteria)
        {
            var searchCriteria = new SearchCriteria
            {
                Name = criteria.Title
            };
            criteria.ReleaseDate.Run(d =>
            {
                var utc = DateTime.SpecifyKind(d, DateTimeKind.Utc);
                searchCriteria.ReleaseDate = Timestamp.FromDateTime(utc);
            });

            var movies = await _moviesClient.SearchByCriteriaAsync(searchCriteria);
            return movies.Movies.Select(m => new MovieModel
            {
                Title = m.Title,
                ReleaseDate = m.ReleaseDate.ToDateTime()
            });
        }
    }
}
