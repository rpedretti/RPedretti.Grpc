using Grpc.Core;
using System;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using System.Linq;
using RPedretti.Grpc.DAL.Accessor;
using RJPSoft.HelperExtensions;
using Microsoft.AspNetCore.Authorization;

namespace RPedretti.Grpc.Server.Services
{
    public class MovieService : Movies.MoviesBase
    {
        private readonly IMoviesAccessor _moviesAccessor;

        public MovieService(IMoviesAccessor moviesAccessor)
        {
            _moviesAccessor = moviesAccessor;
        }

        [Authorize]
        public override async Task<MovieResponse> GetById(IdRequest request, ServerCallContext context)
        {
            var movie = await _moviesAccessor.GetMovieByIdAsync(request.Id);
            return new MovieResponse
            {
                Movie = movie.Let(m => new Movie
                {
                    Title = m.Name,
                    ReleaseDate = Timestamp.FromDateTime(m.ReleaseDate)
                })
            };
        }

        [Authorize(Roles = "Admin")]
        public override async Task<MultipleMoviesReply> SearchByCriteria(SearchCriteria request, ServerCallContext context)
        {
            var movies = await _moviesAccessor.FindMoviesByCriteriaAsync(new FilterCriteria
            {
                Title = request.Name,
                ReleaseDate = request.ReleaseDate?.ToDateTime()
            });

            var reply = new MultipleMoviesReply();
            reply.Movies.AddRange(movies.Select(m => new Movie
            {
                Title = m.Name,
                ReleaseDate = m.ReleaseDate.ToTimestamp()
            }));
            return await Task.FromResult(reply);
        }
    }
}
