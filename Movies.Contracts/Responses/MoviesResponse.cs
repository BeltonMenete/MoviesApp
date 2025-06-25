using Movies.Application.Models;

namespace Movies.Contracts.Responses;

public class MoviesResponse
{
    public required IEnumerable<Movie> Items { get; init; } = Enumerable.Empty<Movie>(); 
}