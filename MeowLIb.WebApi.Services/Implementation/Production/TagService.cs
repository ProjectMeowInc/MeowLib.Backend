using LanguageExt.Common;
using MeowLib.Domain.DbModels.TagEntity;
using MeowLib.Domain.Dto.Tag;
using MeowLib.Domain.Exceptions;
using MeowLib.Domain.Exceptions.DAL;
using MeowLib.Domain.Exceptions.Services;
using MeowLib.Domain.Models;
using MeowLib.WebApi.DAL.Repository.Interfaces;
using MeowLIb.WebApi.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace MeowLIb.WebApi.Services.Implementation.Production;

/// <summary>
/// Сервси для работы с тегами.
/// </summary>
public class TagService : ITagService
{
    private readonly ITagRepository _tagRepository;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="tagRepository">Репозиторий тегов.</param>
    public TagService(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

    /// <summary>
    /// Метод создаёт новый тег.
    /// </summary>
    /// <param name="createTagEntityModel">Данные для создания тега.</param>
    /// <returns>Информацию о созданном теге.</returns>
    /// <exception cref="ValidationException">Возникает в случае ошибки валидации данных.</exception>
    /// <exception cref="ApiException">Возникает в случае ошибки сохранения данных.</exception>
    public async Task<Result<TagEntityModel>> CreateTagAsync(CreateTagEntityModel createTagEntityModel)
    {
        var validationErrors = new List<ValidationErrorModel>();

        if (string.IsNullOrEmpty(createTagEntityModel.Name))
        {
            validationErrors.Add(new ValidationErrorModel
            {
                PropertyName = nameof(createTagEntityModel.Name),
                Message = "Имя не может быть пустым"
            });
        }

        if (createTagEntityModel.Name.Length > 15)
        {
            validationErrors.Add(new ValidationErrorModel
            {
                PropertyName = nameof(createTagEntityModel.Name),
                Message = "Имя не может быть больше 15 символов"
            });
        }

        if (createTagEntityModel.Description is not null)
        {
            if (string.IsNullOrEmpty(createTagEntityModel.Description))
            {
                validationErrors.Add(new ValidationErrorModel
                {
                    PropertyName = nameof(createTagEntityModel.Description),
                    Message = "Описание не может быть пустой строкой"
                });
            }
            
            if (createTagEntityModel.Description.Length > 100)
            {
                validationErrors.Add(new ValidationErrorModel
                {
                    PropertyName = nameof(createTagEntityModel.Description),
                    Message = "Описание не может быть больше 100 символов"
                });
            }
        }

        if (validationErrors.Any())
        {
            var validationException = new ValidationException(nameof(TagService), validationErrors);
            return new Result<TagEntityModel>(validationException);
        }

        var createModel = new CreateTagEntityModel
        {
            Name = createTagEntityModel.Name,
            Description = createTagEntityModel.Description
        };

        try
        {
            return await _tagRepository.CreateAsync(createModel);
        }
        catch (DbUpdateException)
        {
            var apiException = new ApiException("Внутренняя ошибка сервера.");
            return new Result<TagEntityModel>(apiException);
        }
    }

    /// <summary>
    /// Метод получает тег по его Id.
    /// </summary>
    /// <param name="id">Id тега.</param>
    /// <returns>Найденный тег, иначе - null</returns>
    public async Task<TagEntityModel?> GetTagByIdAsync(int id)
    {
        var foundedTag = await _tagRepository.GetByIdAsync(id);
        return foundedTag;
    }

    /// <summary>
    /// Метод получает все теги в формате Dto.
    /// </summary>
    /// <returns>Массив Dto тегов.</returns>
    public async Task<IEnumerable<TagDto>> GetAllTagsAsync()
    {
        var tags = await _tagRepository.GetAll().Select(t => new TagDto
        {
            Id = t.Id,
            Name = t.Name
        }).ToListAsync();

        return tags;
    }

    /// <summary>
    /// Метод удаляет тег по его Id.
    /// </summary>
    /// <param name="id">Id тега.</param>
    /// <returns>True - в случае удачного удаления, false - если тег не был найден.</returns>
    /// <exception cref="ApiException">Возникает в случае ошибки сохранения данных.</exception>
    public async Task<Result<bool>> DeleteTagByIdAsync(int id)
    {
        try
        {
            return await _tagRepository.DeleteByIdAsync(id);
        }
        catch (DbUpdateException)
        {
            var apiException = new ApiException("Внутренняя ошибка сервера");
            return new Result<bool>(apiException);
        }
    }

    /// <summary>
    /// Метод обновляет информацию о теге.
    /// </summary>
    /// <param name="id">Id тега.</param>
    /// <param name="updateTagEntityModel">Данные для обновления.</param>
    /// <returns>Обновлённую информацию о теге или null если тег не был найден.</returns>
    /// <exception cref="ValidationException">Возникает в случае ошибки валидации данных.</exception>
    /// <exception cref="ApiException">Возникает в случае ошибки сохранения данных.</exception>
    public async Task<Result<TagEntityModel?>> UpdateTagByIdAsync(int id, UpdateTagEntityModel updateTagEntityModel)
    {
        var validationErrors = new List<ValidationErrorModel>();
        
        if (updateTagEntityModel.Name is not null)
        {
            if (string.IsNullOrEmpty(updateTagEntityModel.Name))
            {
                validationErrors.Add(new()
                {
                    PropertyName = nameof(updateTagEntityModel.Name),
                    Message = "Имя тега не может быть пустой строкой"
                });
            }

            if (updateTagEntityModel.Name.Length > 15)
            {
                validationErrors.Add(new ValidationErrorModel
                {
                    PropertyName = nameof(updateTagEntityModel.Name),
                    Message = "Имя тега не можеь быть больше 15 символов"
                });
            }
        }

        if (updateTagEntityModel.Description is not null)
        {
            if (string.IsNullOrEmpty(updateTagEntityModel.Description))
            {
                validationErrors.Add(new ValidationErrorModel
                {
                    PropertyName = nameof(updateTagEntityModel.Description),
                    Message = "Описание не может быть пустой строкой"
                });
            }
            
            if (updateTagEntityModel.Description.Length > 256)
            {
                validationErrors.Add(new ValidationErrorModel
                {
                    PropertyName = nameof(updateTagEntityModel.Description),
                    Message = "Описание не может быть больше 256 символов"
                });
            }
        }

        if (validationErrors.Any())
        {
            var validationException = new ValidationException(nameof(TagService), validationErrors);
            return new Result<TagEntityModel?>(validationException);
        }
        
        try
        {
            return await _tagRepository.UpdateAsync(id, updateTagEntityModel);
        }
        catch (EntityNotFoundException)
        {
            return null;
        }
        catch (DbUpdateException)
        {
            var apiException = new ApiException("Внутренняя ошибка сервера");
            return new Result<TagEntityModel?>(apiException);
        }
    }
}