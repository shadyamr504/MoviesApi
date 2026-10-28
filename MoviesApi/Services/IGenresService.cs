namespace MoviesApi.Services
{
    public interface IGenresService
    {
        Task<IEnumerable<Genre>> GetAll();
        Task<Genre?> GetById(byte id);
        Task<Genre> Add(Genre genre);
        Genre Update(Genre genre);
        Task<bool> TryDeleteAsync(Genre genre);
        Task<bool> IsValidGenre(byte id);
    }
}
