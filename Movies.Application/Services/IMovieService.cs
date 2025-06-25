using Movies.Application.Models;

namespace Movies.Application.Services;

public interface IMovieService
{
    Task<bool> CreateByIdAsync(Movie movie);
    Task<Movie?> GetByIdAsync(Guid id);
    Task<Movie?> GetBySlugAsync(string slug);
    Task<IEnumerable<Movie>> GetAllAsync();
    Task<bool> UpdateAsync(Movie movie);
    Task<bool> DeleteByIdAsync(Guid id);
}