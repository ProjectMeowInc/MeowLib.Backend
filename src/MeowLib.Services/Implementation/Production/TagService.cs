using MeowLib.DAL;
using MeowLib.Domain.Shared.Exceptions;
using MeowLib.Domain.Shared.Models;
using MeowLib.Domain.Shared.Result;
using MeowLib.Domain.Tag.Dto;
using MeowLib.Domain.Tag.Entity;
using MeowLib.Domain.Tag.Services;
using Microsoft.EntityFrameworkCore;

namespace MeowLib.Services.Implementation.Production;

/// <summary>
/// Сервси для работы с тегами.
/// </summary>
public class TagService(ApplicationDbContext dbContext) : ITagService
{
    public async Task<Result<TagEntityModel>> CreateTagAsync(string name, string? description)
    {
        var validationErrors = new List<ValidationErrorModel>();

        if (string.IsNullOrEmpty(name))
        {
            validationErrors.Add(new ValidationErrorModel
            {
                PropertyName = nameof(name),
                Message = "Имя не может быть пустым"
            });
        }

        if (name.Length > 15)
        {
            validationErrors.Add(new ValidationErrorModel
            {
                PropertyName = nameof(name),
                Message = "Имя не может быть больше 15 символов"
            });
        }

        if (description is not null)
        {
            if (string.IsNullOrEmpty(description))
            {
                validationErrors.Add(new ValidationErrorModel
                {
                    PropertyName = nameof(description),
                    Message = "Описание не может быть пустой строкой"
                });
            }

            if (description.Length > 100)
            {
                validationErrors.Add(new ValidationErrorModel
                {
                    PropertyName = nameof(description),
                    Message = "Описание не может быть больше 100 символов"
                });
            }
        }

        if (validationErrors.Any())
        {
            var validationException = new ValidationException(validationErrors);
            return Result<TagEntityModel>.Fail(validationException);
        }

        var entry = await dbContext.Tags.AddAsync(new TagEntityModel
        {
            Name = name,
            Description = description ?? "",
            Books = []
        });

        await dbContext.SaveChangesAsync();
        return Result<TagEntityModel>.Ok(entry.Entity);
    }

    public Task<TagEntityModel?> GetTagByIdAsync(int id)
    {
        return dbContext.Tags.FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<TagDto>> GetAllTagsAsync()
    {
        var tags = await dbContext.Tags
            .Select(t => new TagDto
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description
            }).ToListAsync();

        return tags;
    }

    public async Task<bool> DeleteTagByIdAsync(int id)
    {
        var foundedTag = await GetTagByIdAsync(id);
        if (foundedTag is null)
        {
            return false;
        }

        dbContext.Tags.Remove(foundedTag);
        await dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<Result<TagEntityModel?>> UpdateTagByIdAsync(int id, string? name, string? description)
    {
        var validationErrors = new List<ValidationErrorModel>();

        if (name is not null)
        {
            if (string.IsNullOrEmpty(name))
            {
                validationErrors.Add(new ValidationErrorModel
                {
                    PropertyName = nameof(name),
                    Message = "Имя тега не может быть пустой строкой"
                });
            }

            if (name.Length > 15)
            {
                validationErrors.Add(new ValidationErrorModel
                {
                    PropertyName = nameof(name),
                    Message = "Имя тега не можеь быть больше 15 символов"
                });
            }
        }

        if (description is not null)
        {
            if (string.IsNullOrEmpty(description))
            {
                validationErrors.Add(new ValidationErrorModel
                {
                    PropertyName = nameof(description),
                    Message = "Описание не может быть пустой строкой"
                });
            }

            if (description.Length > 256)
            {
                validationErrors.Add(new ValidationErrorModel
                {
                    PropertyName = nameof(description),
                    Message = "Описание не может быть больше 256 символов"
                });
            }
        }

        if (validationErrors.Any())
        {
            var validationException = new ValidationException(validationErrors);
            return Result<TagEntityModel?>.Fail(validationException);
        }

        var foundedTag = await GetTagByIdAsync(id);
        if (foundedTag is null)
        {
            return Result<TagEntityModel?>.Ok(null);
        }


        foundedTag.Name = name ?? foundedTag.Name;
        foundedTag.Description = description ?? foundedTag.Description;

        dbContext.Tags.Update(foundedTag);
        await dbContext.SaveChangesAsync();

        return Result<TagEntityModel?>.Ok(foundedTag);
    }
}