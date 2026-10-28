using Microsoft.EntityFrameworkCore;
namespace MoviesApi.Services
{
    public class GenresService : IGenresService
    {
        private readonly ApplicationDbContext _context;

        public GenresService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Genre> Add(Genre genre)
        {
            await _context.AddAsync(genre);
            await _context.SaveChangesAsync();
            return genre;
        }



        public async Task<bool> TryDeleteAsync(Genre genre)
        {
            var hasMovies = await _context.Movies
                .AnyAsync(m => m.GenreId == genre.Id);

            if (hasMovies)
            {
                return false;
            }

            _context.Genres.Remove(genre);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<Genre>> GetAll()
        {
            return await _context.Genres.OrderBy(g => g.Name).ToListAsync();
        }

        public async Task<Genre?> GetById(byte id)
        {
            return await _context.Genres.SingleOrDefaultAsync(g => g.Id == id);
        }

        public Task<bool> IsValidGenre(byte id)
        {
            return _context.Genres.AnyAsync(g => g.Id == id);
        }

        public Genre Update(Genre genre)
        {
            _context.Update(genre);
            _context.SaveChanges();
            return genre;
        }
    }
}
