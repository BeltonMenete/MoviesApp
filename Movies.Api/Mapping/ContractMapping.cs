﻿using Movies.Application.Models;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api.Mapping;

public static class ContractMapping
{
    public static Movie MapToMovie(this CreateMovieRequest request)
    {
        return new Movie()
        {
            Id = Guid.NewGuid(),
            Title = request.Title.Trim(),
            ReleaseYear = request.ReleaseYear,
            Genres = request.Genres.ToList()
        };
    }

    public static Movie MapToMovie(this UpdateMovieRequest request, Guid id)
    {
        return new Movie()
        {
            Id = id,
            Title = request.Title.Trim(),
            ReleaseYear = request.ReleaseYear,
            Genres = request.Genres.ToList()
        };
    }

    public static MovieResponse MapToResponse(this Movie movie)
    {
        return new MovieResponse()
        {
            Id = movie.Id,
            Title = movie.Title,
            Slug = movie.Slug,
            ReleaseYear = movie.ReleaseYear,
            Genres = movie.Genres.ToList()
        };
    }
}