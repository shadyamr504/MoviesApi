using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Services;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMoviesService _moviesService;
        private readonly IGenresService _genresService;

        private List<string> _allowedExtenstion = new List<string> { ".jpg", ".png" };
        private long _maxAllowedPosterSize = 1048576;
        public MoviesController(IMoviesService moviesService, IGenresService genresService, IMapper mapper)
        {
            _moviesService = moviesService;
            _genresService = genresService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var movies = await _moviesService.GetAll();
            var data = _mapper.Map<IEnumerable<MovieDetailsDto>>(movies);
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var movie = await _moviesService.GetById(id);

            if (movie == null)
                return NotFound();
            var movieDetails = _mapper.Map<MovieDetailsDto>(movie);
            return Ok(movieDetails);
        }

        [HttpGet("GetByGenreId")]
        public async Task<IActionResult> GetByGenreIdAsync(byte GenreId)
        {
            var movies = await _moviesService.GetAll(GenreId);
            var data = _mapper.Map<IEnumerable<MovieDetailsDto>>(movies);
            return Ok(data);
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
            var IsValidGenre = await _genresService.IsValidGenre(dto.GenreId);
            if (!IsValidGenre)
                return BadRequest("Invalid genre ID");
            using var dataStream = new MemoryStream();
            await dto.Poster.CopyToAsync(dataStream);
            var movie = _mapper.Map<Movie>(dto);
            movie.Poster = dataStream.ToArray();
            await _moviesService.Add(movie);
            return Ok(movie);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromForm] MovieDto dto)
        {
            var movie = await _moviesService.GetById(id);
            if (movie == null)
                return NotFound("No Movie found with ID " + id);
            var IsValidGenre = await _genresService.IsValidGenre(dto.GenreId);
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
            _moviesService.Update(movie);
            return Ok(movie);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var movie = await _moviesService.GetById(id);
            if (movie == null)
                return NotFound("No Movie found with ID " + id);
            _moviesService.Delete(movie);
            return Ok(movie);
        }
    }
}