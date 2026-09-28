using System;
using System.Collections.Generic;
using System.Text;

namespace CineVerse.Application.DTOs.Responses;

public sealed record TagResponse(
    Guid Id,
    string Name,
    string Slug
    );
