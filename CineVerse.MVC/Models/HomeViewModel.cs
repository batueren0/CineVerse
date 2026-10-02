using CineVerse.Application.DTOs.Responses;

namespace CineVerse.MVC.Models;

public record HomeViewModel(
    IEnumerable<MovieResponse> Carousel,
    IEnumerable<MovieResponse> InTheaters,
    IEnumerable<MovieResponse> Latest,
    IEnumerable<MovieResponse> Recent
    );
