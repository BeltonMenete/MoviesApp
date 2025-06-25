using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Services;

public class MovieService(IMovieRepository movieRepository) : IMovieService
{
    private readonly IMovieRepository _movieRepository = movieRepository;
    private readonly IValidator<Movie> _movieValidator;

    public async Task<bool> CreateByIdAsync(Movie movie)
    {
        await _movieValidator.ValidateAndThrowAsync(movie);
        return await _movieRepository.CreateByIdAsync(movie);
    }

    public Task<Movie?> GetByIdAsync(Guid id)
    {
        return _movieRepository.GetByIdAsync(id);
    }

    public Task<Movie?> GetBySlugAsync(string slug)
    {
        return _movieRepository.GetBySlugAsync(slug);
    }

    public Task<IEnumerable<Movie>> GetAllAsync()
    {
        return _movieRepository.GetAllAsync();
    }

    public async Task<bool> UpdateAsync(Movie movie)
    {
        await _movieValidator.ValidateAndThrowAsync(movie);
        var movieExists = await _movieRepository.ExistsByIdAsync(movie.Id);

        if (!movieExists)
        {
            return false;
        }

        await _movieRepository.UpdateAsync(movie);
        return true;
    }

    public Task<bool> DeleteByIdAsync(Guid id)
    {
        return _movieRepository.DeleteByIdAsync(id);
    }
}