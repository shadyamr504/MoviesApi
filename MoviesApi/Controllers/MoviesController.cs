using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private new List<string> _allowedExtenstion = new List<string> { ".jpg", ".png" };
        private long _maxAllowedPosterSize = 1048576;
        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var movies = await _context.Movies
                .OrderByDescending(m => m.Rate)
                .Include(m => m.Genre)
                .Select(m => new MovieDetailsDto
                {
                    Id = m.Id,
                    Title = m.Title,
                    Year = m.Year,
                    Rate = m.Rate,
                    StoryLine = m.StoryLine,
                    Poster = m.Poster,
                    GenreId = m.GenreId,
                    GenreName = m.Genre.Name
                })
                .ToListAsync();
            return Ok(movies);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var movie = await _context.Movies
                .Include(m => m.Genre)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (movie == null)
                return NotFound();
            var movieDetails = new MovieDetailsDto
            {
                Id = movie.Id,
                Title = movie.Title,
                Year = movie.Year,
                Rate = movie.Rate,
                StoryLine = movie.StoryLine,
                Poster = movie.Poster,
                GenreId = movie.GenreId,
                GenreName = movie.Genre.Name
            };
            return Ok(movieDetails);
        }

        [HttpGet("GetByGenreId")]
        public async Task<IActionResult> GetByGenreIdAsync(byte GenreId)
        {
            var movies = await _context.Movies
                .Where(m => m.GenreId == GenreId)
                .OrderByDescending(m => m.Rate)
                .Include(m => m.Genre)
                .Select(m => new MovieDetailsDto
                {
                    Id = m.Id,
                    Title = m.Title,
                    Year = m.Year,
                    Rate = m.Rate,
                    StoryLine = m.StoryLine,
                    Poster = m.Poster,
                    GenreId = m.GenreId,
                    GenreName = m.Genre.Name
                })
                .ToListAsync();
            return Ok(movies);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromForm] MovieDto dto)
        {
            if (dto.Poster == null)
                return BadRequest("Poster is required");
            if (!_allowedExtenstion.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
                return BadRequest("Only .png and .jpg images are allowed");
            if (dto.Poster.Length > _maxAllowedPosterSize)
                return BadRequest("Poster size cannot exceed 1MB");
            var IsValidGenre = await _context.Genres.AnyAsync(g => g.Id == dto.GenreId);
            if (!IsValidGenre)
                return BadRequest("Invalid genre ID");
            using var dataStream = new MemoryStream();
            await dto.Poster.CopyToAsync(dataStream);
            var movie = new Movie
            {
                Title = dto.Title,
                Year = dto.Year,
                Rate = dto.Rate,
                StoryLine = dto.StoryLine,
                Poster = dataStream.ToArray(),
                GenreId = dto.GenreId
            };
            await _context.Movies.AddAsync(movie);
            await _context.SaveChangesAsync();
            return Ok(movie);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromForm] MovieDto dto)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
                return NotFound("No Movie found with ID " + id);
            var IsValidGenre = await _context.Genres.AnyAsync(g => g.Id == dto.GenreId);
            if (!IsValidGenre)
                return BadRequest("Invalid genre ID");
            movie.Title = dto.Title;
            movie.Year = dto.Year;
            movie.Rate = dto.Rate;
            movie.StoryLine = dto.StoryLine;
            if (dto.Poster != null)
            {
                if (!_allowedExtenstion.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
                    return BadRequest("Only .png and .jpg images are allowed");
                if (dto.Poster.Length > _maxAllowedPosterSize)
                    return BadRequest("Poster size cannot exceed 1MB");
                using var dataStream = new MemoryStream();
                await dto.Poster.CopyToAsync(dataStream);
                movie.Poster = dataStream.ToArray();
            }
            movie.GenreId = dto.GenreId;
            await _context.SaveChangesAsync();
            return Ok(movie);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
                return NotFound("No Movie found with ID " + id);
            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();
            return Ok(movie);
        }
    }
}