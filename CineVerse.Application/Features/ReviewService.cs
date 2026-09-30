using CineVerse.Application.Common;
using CineVerse.Application.Contracts.Services;
using CineVerse.Application.DTOs.Requests;
using CineVerse.Application.DTOs.Responses;
using CineVerse.Domain.Entities;
using FluentValidation;

namespace CineVerse.Application.Features;

public class ReviewService(IUnitOfWork uow,
    IValidator<CreateReviewRequest> validator
    ) : IReviewService
{
    public async Task<Result> CreateAsync(CreateReviewRequest request)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return Result.Failure(string.Join(",", validationResult.Errors.Select(e => e.ErrorMessage)));

        var review = new Review 
        { 
            Id = Guid.CreateVersion7(),
            MovieId = request.MovieId,
            UserId = request.UserId,
            Rating = request.Rating,
            Body = request.Body,
        };

        await uow.Reviews.AddAsync(review);
        await uow.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var review = await uow.Reviews.GetByIdAsync(id);
        if (review is null)
            return Result.Failure("Comment was not found");

        review.IsActive = false;
        review.DeletedOn = DateTimeOffset.UtcNow;

        uow.Reviews.Delete(review);
        await uow.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<IEnumerable<ReviewResponse>> GetAllAsync()
    {
        var reviews = await uow.Reviews.GetAllAsync();

        return reviews.Select(r => new ReviewResponse(
             r.Id,
             r.MovieId,
             r.Movie?.Title ?? string.Empty,
             r.User?.UserName ?? string.Empty,
             r.Rating,
             r.Body,
             r.CreatedOn
             ));
    }
}
