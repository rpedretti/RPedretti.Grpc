using Grpc.Core;
using System;
using System.Threading;

namespace RPedretti.Grpc.Client.Shared.Interfaces
{
    public interface IMoviesClient
    {
        MovieResponse GetById(IdRequest request, Metadata? headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default);
        MovieResponse GetById(IdRequest request, CallOptions options);
        AsyncUnaryCall<MovieResponse> GetByIdAsync(IdRequest request, Metadata? headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default);
        AsyncUnaryCall<MovieResponse> GetByIdAsync(IdRequest request, CallOptions options);
        MultipleMoviesReply SearchByCriteria(SearchCriteria request, Metadata? headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default);
        MultipleMoviesReply SearchByCriteria(SearchCriteria request, CallOptions options);
        AsyncUnaryCall<MultipleMoviesReply> SearchByCriteriaAsync(SearchCriteria request, Metadata? headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default);
        AsyncUnaryCall<MultipleMoviesReply> SearchByCriteriaAsync(SearchCriteria request, CallOptions options);
    }
}
