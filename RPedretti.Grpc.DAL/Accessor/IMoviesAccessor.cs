using RPedretti.Grpc.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RPedretti.Grpc.DAL.Accessor
{
    public interface IMoviesAccessor
    {
        Task<IEnumerable<Movie>> FindMoviesByCriteriaAsync(FilterCriteria criteria);
        Task<Movie?> GetMovieByIdAsync(int id);
    }
}