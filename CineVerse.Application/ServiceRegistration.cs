
using CineVerse.Application.Contracts.Services;
using CineVerse.Application.Features;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CineVerse.Application;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(ServiceRegistration).Assembly);

        services.AddScoped<IGenreService, GenreService>();
        services.AddScoped<ITagService, TagService>();
        services.AddScoped<IMovieTagService, MovieTagService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<IMovieService, MovieService>();

        return services;
    }
}
