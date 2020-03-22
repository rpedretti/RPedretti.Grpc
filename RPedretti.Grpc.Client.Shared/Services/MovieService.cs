using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using RPedretti.Grpc.Client.Shared.Interfaces;
using RPedretti.Grpc.Client.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPedretti.Grpc.Client.Shared.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMoviesClient _moviesClient;
        private readonly ISecurityService _securityService;
        private string? _token;

        public MovieService(IMoviesClient moviesClient, ISecurityService securityService)
        {
            _moviesClient = moviesClient;
            _securityService = securityService;
        }

        public async Task<IEnumerable<MovieModel>> FindByCriteriaAsync(Models.SearchCriteria criteria)
        {
            var searchCriteria = new SearchCriteria
            {
                Name = criteria.Title
            };
            if (criteria.ReleaseDate is DateTime d) 
            {
                var utc = DateTime.SpecifyKind(d, DateTimeKind.Utc);
                searchCriteria.ReleaseDate = Timestamp.FromDateTime(utc);
            };

            var headers = await CreateHeaders(criteria.Title != null ? "admin" : "user");

            var movies = await _moviesClient.SearchByCriteriaAsync(searchCriteria, headers);

            return movies.Movies.Select(m => new MovieModel
            {
                Title = m.Title,
                ReleaseDate = m.ReleaseDate.ToDateTime()
            });
        }

        private async Task<Metadata> CreateHeaders(string name)
        {
            _token ??= await _securityService.Login(name);
            var metadata = new Metadata
            {
                new Metadata.Entry("Authorization", $"Bearer {_token}")
            };
            return metadata;
        }
    }
}
