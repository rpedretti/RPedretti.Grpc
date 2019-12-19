using RPedretti.Grpc.DAL.Context;
using RPedretti.Grpc.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace RPedretti.Grpc.DAL.Accessor
{
    public class MoviesAccessor : IMoviesAccessor
    {
        private readonly MoviesContext _context;

        public MoviesAccessor(MoviesContext context)
        {
            _context = context;
        }

        public async Task<Movie?> GetMovieByIdAsync(int id)
        {
            return await _context.Movies.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<Movie>> FindMoviesByCriteriaAsync(FilterCriteria criteria)
        {
            IEnumerable<Movie> result = Enumerable.Empty<Movie>(); 
            if (criteria.IsValid())
            {
                IQueryable<Movie>? query = null;
                if (!string.IsNullOrEmpty(criteria.Title))
                {
                    query = _context.Movies.Where(m => EF.Functions.Like(m.Name, $"%{criteria.Title}%"));
                }
                if (criteria.ReleaseDate.HasValue)
                {
                    query = (query ?? _context.Movies).Where(m => m.ReleaseDate == criteria.ReleaseDate);
                }

                result = await query.ToListAsync();
            }

            return result;
        }
    }
}
