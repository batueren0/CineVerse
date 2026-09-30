using System;
using System.Collections.Generic;
using System.Text;

namespace CineVerse.Application.DTOs.Responses;

public sealed record ReviewResponse(
    Guid Id,
    Guid MovieId,
    string MovieTitle,
    string UserName,
    int Rating,
    string Body,
    DateTimeOffset CreatedOn
    );
