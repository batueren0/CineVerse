using System;
using System.Collections.Generic;
using System.Text;

namespace CineVerse.Application.DTOs.Responses;

public sealed record MovieResponse(
    Guid Id,
    string Title,
    string Slug,
    string Director,
    int ReleaseYear,
    string? PosterUrl,
    string Overview,
    DateTimeOffset LastModifiedAt,
    GenreResponse Genre,
    IEnumerable<TagResponse> Tags,
    IEnumerable<ReviewResponse> Reviews
    );
