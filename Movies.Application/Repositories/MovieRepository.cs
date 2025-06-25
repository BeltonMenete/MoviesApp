using Dapper;
using Movies.Application.Database;
using Movies.Application.Models;

namespace Movies.Application.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly IDbConnectionFactory _dbFactory;

    public MovieRepository(IDbConnectionFactory dbFactory) => _dbFactory = dbFactory;

    public async Task<bool> CreateByIdAsync(Movie movie)
    {
        using var connection = await _dbFactory.CreateConnectionAsync();
        using var tx = connection.BeginTransaction();

        try
        {
            const string insertMovie = """
                INSERT INTO Movies (Id, Title, Slug, ReleaseYear)
                VALUES (@Id, @Title, @Slug, @ReleaseYear);
            """;

            const string insertGenre = """
                INSERT INTO Genres (MovieId, Name)
                VALUES (@MovieId, @Name);
            """;

            await connection.ExecuteAsync(insertMovie, new
            {
                movie.Id,
                movie.Title,
                movie.Slug,
                movie.ReleaseYear
            }, tx);

            var genreParams = movie.Genres.Select(g => new { MovieId = movie.Id, Name = g });
            await connection.ExecuteAsync(insertGenre, genreParams, tx);

            tx.Commit();
            return true;
        }
        catch
        {
            tx.Rollback();
            return false;
        }
    }

    public async Task<Movie?> GetByIdAsync(Guid id)
    {
        using var connection = await _dbFactory.CreateConnectionAsync();

        const string sql = """
            SELECT * FROM Movies WHERE Id = @Id;
            SELECT Name FROM Genres WHERE MovieId = @Id;
        """;

        using var multi = await connection.QueryMultipleAsync(sql, new { Id = id });

        var movie = await multi.ReadSingleOrDefaultAsync<Movie>();
        if (movie is null) return null;

        movie.Genres = (await multi.ReadAsync<string>()).ToList();
        return movie;
    }

    public async Task<Movie?> GetBySlugAsync(string slug)
    {
        using var connection = await _dbFactory.CreateConnectionAsync();

        const string sql = """
            SELECT * FROM Movies WHERE Slug = @Slug;
            SELECT Name FROM Genres WHERE MovieId = (SELECT Id FROM Movies WHERE Slug = @Slug);
        """;

        using var multi = await connection.QueryMultipleAsync(sql, new { Slug = slug });

        var movie = await multi.ReadSingleOrDefaultAsync<Movie>();
        if (movie is null) return null;

        movie.Genres = (await multi.ReadAsync<string>()).ToList();
        return movie;
    }

    public async Task<IEnumerable<Movie>> GetAllAsync()
    {
        using var connection = await _dbFactory.CreateConnectionAsync();

        const string sql = """
            SELECT * FROM Movies;
            SELECT MovieId, Name FROM Genres;
        """;

        using var multi = await connection.QueryMultipleAsync(sql);

        var movies = (await multi.ReadAsync<Movie>()).ToList();
        var genres = (await multi.ReadAsync<(Guid MovieId, string Name)>()).ToList();

        foreach (var movie in movies)
        {
            movie.Genres = genres
                .Where(g => g.MovieId == movie.Id)
                .Select(g => g.Name)
                .ToList();
        }

        return movies;
    }

    public async Task<bool> UpdateAsync(Movie movie)
    {
        using var connection = await _dbFactory.CreateConnectionAsync();
        using var tx = connection.BeginTransaction();

        try
        {
            const string updateMovie = """
                UPDATE Movies SET Title = @Title, Slug = @Slug, ReleaseYear = @ReleaseYear
                WHERE Id = @Id;
            """;

            const string deleteGenres = "DELETE FROM Genres WHERE MovieId = @MovieId;";
            const string insertGenre = "INSERT INTO Genres (MovieId, Name) VALUES (@MovieId, @Name);";

            await connection.ExecuteAsync(updateMovie, new
            {
                movie.Id,
                movie.Title,
                movie.Slug,
                movie.ReleaseYear
            }, tx);

            await connection.ExecuteAsync(deleteGenres, new { MovieId = movie.Id }, tx);

            var genreParams = movie.Genres.Select(g => new { MovieId = movie.Id, Name = g });
            await connection.ExecuteAsync(insertGenre, genreParams, tx);

            tx.Commit();
            return true;
        }
        catch
        {
            tx.Rollback();
            return false;
        }
    }

    public async Task<bool> DeleteByIdAsync(Guid id)
    {
        using var connection = await _dbFactory.CreateConnectionAsync();
        using var tx = connection.BeginTransaction();

        try
        {
            const string deleteGenres = "DELETE FROM Genres WHERE MovieId = @Id;";
            const string deleteMovie = "DELETE FROM Movies WHERE Id = @Id;";

            await connection.ExecuteAsync(deleteGenres, new { Id = id }, tx);
            await connection.ExecuteAsync(deleteMovie, new { Id = id }, tx);

            tx.Commit();
            return true;
        }
        catch
        {
            tx.Rollback();
            return false;
        }
    }

    public async Task<bool> ExistsByIdAsync(Guid id)
    {
        using var connection = await _dbFactory.CreateConnectionAsync();

        const string sql = "SELECT 1 FROM Movies WHERE Id = @Id;";
        return (await connection.ExecuteScalarAsync<int?>(sql, new { Id = id })).HasValue;
    }
}
