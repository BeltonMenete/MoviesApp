using Microsoft.AspNetCore.Mvc;
using Movies.Api.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests;

namespace Movies.Api.Controllers;

[ApiController]
public class MoviesController(IMovieService db) : ControllerBase
{
    [HttpGet(ApiEndpoints.Movies.Get)]
    public async Task<IActionResult> Get([FromRoute] string idOrSlug)
    {
        var movie = Guid.TryParse(idOrSlug, out var id)
            ? await db.GetByIdAsync(id)
            : await db.GetBySlugAsync(idOrSlug);

        if (movie is null) return NotFound();
        var response = movie.MapToResponse();
        return Ok(response);
    }

    [HttpGet(ApiEndpoints.Movies.GetAll)]
    public async Task<IActionResult> GetAll()
    {
        var movies = await db.GetAllAsync();
        var response = movies.Select(movie => movie.MapToResponse());
        return Ok(response);
    }

    [HttpPost(ApiEndpoints.Movies.Create)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateMovieRequest request)
    {
        var movie = request.MapToMovie();
        await db.CreateByIdAsync(movie);
        return CreatedAtAction(nameof(Get), new { idOrSlug = movie.Id }, movie.MapToResponse());
    }

    [HttpPut(ApiEndpoints.Movies.Update)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateMovieRequest request)
    {
        var updated = await db.UpdateAsync(request.MapToMovie(id));
        if (!updated)
            return NotFound();

        var response = request.MapToMovie(id);
        return Ok(response);
    }

    [HttpDelete(ApiEndpoints.Movies.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var isDeleted = await db.DeleteByIdAsync(id);
        if (!isDeleted)
            return NotFound();
        return NoContent();
    }
}