using RPedretti.Grpc.Client.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RPedretti.Grpc.Client.Shared.Services
{
    public interface IMovieService
    {
        Task<IEnumerable<MovieModel>> FindByCriteria(Models.SearchCriteria criteria);
    }
}
