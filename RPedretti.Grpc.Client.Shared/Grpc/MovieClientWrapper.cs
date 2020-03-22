using Grpc.Core;
using RPedretti.Grpc.Client.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace RPedretti.Grpc.Client.Shared.Grpc
{
    public class MovieClientWrapper : IMoviesClient
    {
        private readonly Movies.MoviesClient _moviesClient;

        public MovieClientWrapper(Movies.MoviesClient moviesClient)
        {
            _moviesClient = moviesClient;
        }

        public MovieResponse GetById(IdRequest request, Metadata? headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default) =>
            _moviesClient.GetById(request, headers, deadline, cancellationToken);

        public MovieResponse GetById(IdRequest request, CallOptions options) =>
            _moviesClient.GetById(request, options);

        public AsyncUnaryCall<MovieResponse> GetByIdAsync(IdRequest request, Metadata? headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default) =>
            _moviesClient.GetByIdAsync(request, headers, deadline, cancellationToken);

        public AsyncUnaryCall<MovieResponse> GetByIdAsync(IdRequest request, CallOptions options) =>
            _moviesClient.GetByIdAsync(request, options);

        public MultipleMoviesReply SearchByCriteria(SearchCriteria request, Metadata? headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default) =>
            _moviesClient.SearchByCriteria(request, headers, deadline, cancellationToken);

        public MultipleMoviesReply SearchByCriteria(SearchCriteria request, CallOptions options) =>
            _moviesClient.SearchByCriteria(request, options);

        public AsyncUnaryCall<MultipleMoviesReply> SearchByCriteriaAsync(SearchCriteria request, Metadata? headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default) =>
            _moviesClient.SearchByCriteriaAsync(request, headers, deadline, cancellationToken);

        public AsyncUnaryCall<MultipleMoviesReply> SearchByCriteriaAsync(SearchCriteria request, CallOptions options) =>
            _moviesClient.SearchByCriteriaAsync(request, options);
    }
}
