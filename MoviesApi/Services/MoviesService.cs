using Microsoft.EntityFrameworkCore;

namespace MoviesApi.Services
{
    public class MoviesService : IMoviesService
    {
        private readonly ApplicationDbContext _context;
        public MoviesService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Movie> Add(Movie movie)
        {
            await _context.Movies.AddAsync(movie);
            await _context.SaveChangesAsync();
            return movie;
        }

        public Movie Delete(Movie movie)
        {
            _context.Remove(movie);
            _context.SaveChanges();
            return movie;
        }

        public async Task<IEnumerable<Movie>> GetAll(byte genreId = 0)
        {
            return await _context.Movies
                .Where(m => genreId == 0 || m.GenreId == genreId)
                .OrderByDescending(m => m.Rate)
                .Include(m => m.Genre)
                .ToListAsync();
        }

        public async Task<Movie?> GetById(int id)
        {
            return await _context.Movies
                .Include(m => m.Genre)
                .SingleOrDefaultAsync(m => m.Id == id);
        }

        public Movie Update(Movie movie)
        {
            _context.Update(movie);
            _context.SaveChanges();
            return movie;
        }
    }
}