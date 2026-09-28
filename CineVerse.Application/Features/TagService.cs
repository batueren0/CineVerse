
using CineVerse.Application.Common;
using CineVerse.Application.Contracts.Services;
using CineVerse.Application.DTOs.Requests;
using CineVerse.Application.DTOs.Responses;
using CineVerse.Application.Validators;
using FluentValidation;

namespace CineVerse.Application.Features;

public class TagService(
    IUnitOfWork uow,
    IValidator<CreateTagRequest> createValidator,
    IValidator<UpdateTagRequest> updateValidator
    ) : ITagService
{
    public async Task<Result> CreateAsync(CreateTagRequest request)
    {
        var validationResult = await createValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return Result.Failure(string.Join(",", validationResult.Errors.Select(e => e.ErrorMessage)));

        await uow.Tags.AddAsync(new()
        {
            Name = request.Name,
            Slug = request.Name.ToKebabCase()
        });

        await uow.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> UpdateAsync(UpdateTagRequest request)
    {
        var validationResult = await updateValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return Result.Failure(string.Join(",",validationResult.Errors.Select(e => e.ErrorMessage)));


        var tag = await uow.Tags.GetByIdAsync(request.Id);
        if (tag is null)
            return Result.Failure("Tag not found");

        tag.Name = request.Name;
        tag.Slug = request.Name.ToKebabCase();
        tag.LastModifiedOn = DateTime.UtcNow;

        uow.Tags.Update(tag);

        await uow.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var tag = await uow.Tags.GetByIdAsync(id);
        if (tag is null)
            return Result.Failure("Tag not found");

        tag.IsActive = false;
        tag.DeletedOn = DateTime.UtcNow;

        uow.Tags.Delete(tag);
        await uow.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<TagResponse?> GetByIdAsync(Guid id)
    {
        var tag = await uow.Tags.GetByIdAsync(id);
        if (tag is null)
            return default;

        return new TagResponse(
            tag.Id,
            tag.Name,
            tag.Slug
            );
    }



    public async Task<IEnumerable<TagResponse>> GetAllAsync()
    {
        var tags = await uow.Tags.GetAllAsync();
        if (!tags.Any())
            return [];

        return tags.Select(t => new TagResponse(
            t.Id,
            t.Name,
            t.Slug
            )).ToList();
    }



}
