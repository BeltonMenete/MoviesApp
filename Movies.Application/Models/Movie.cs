namespace Movies.Application.Models;

public class Movie
{
    public Guid Id { get; init; }
    public string Title { get; init; } = default!;
    public int ReleaseYear { get; init; }
    public List<string> Genres { get; set; } = new(); 
    public string Slug => GenerateSlug();

    private string GenerateSlug() =>
        $"{Title.Replace(' ', '-').TrimStart()}-{ReleaseYear}".ToLower();
}
