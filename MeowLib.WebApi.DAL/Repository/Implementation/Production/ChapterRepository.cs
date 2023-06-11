﻿using LanguageExt;
using LanguageExt.Common;
using MeowLib.Domain.DbModels.BookEntity;
using MeowLib.Domain.DbModels.ChapterEntity;
using MeowLib.Domain.Enums;
using MeowLib.Domain.Exceptions.DAL;
using MeowLib.WebApi.DAL.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MeowLib.WebApi.DAL.Repository.Implementation.Production;

/// <summary>
/// Репозиторий для работы с главами.
/// </summary>
public class ChapterRepository : IChapterRepository
{
    private readonly ApplicationDbContext _applicationDbContext;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="applicationDbContext">Контекст базы данных.</param>
    public ChapterRepository(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    /// <summary>
    /// Метод создаёт новую главу в базе данных.
    /// </summary>
    /// <param name="chapter">Модель главы для создания.</param>
    /// <returns>Модель созданной книги.</returns>
    /// <exception cref="DbSavingException">Возникает в случае ошибки сохранения данных.</exception>
    public async Task<Result<ChapterEntityModel>> CreateAsync(ChapterEntityModel chapter)
    {
        var createdChapter = await _applicationDbContext.Chapters.AddAsync(chapter);

        try
        {
            await _applicationDbContext.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            var dbSavingException = new DbSavingException(nameof(ChapterEntityModel), DbSavingTypesEnum.Create);
            return new Result<ChapterEntityModel>(dbSavingException);
        }

        return createdChapter.Entity;
    }

    public async Task<ChapterEntityModel?> GetByIdAsync(int chapterId)
    {
        return await _applicationDbContext.Chapters.FirstOrDefaultAsync(c => c.Id == chapterId);
    }

    public async Task<Option<Exception>> DeleteAsync(ChapterEntityModel chapter)
    {
        try
        {
            _applicationDbContext.Chapters.Remove(chapter);
            await _applicationDbContext.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            var dbSavingException = new DbSavingException(nameof(ChapterEntityModel), DbSavingTypesEnum.Delete);
            return Option<Exception>.Some(dbSavingException);
        }
        
        return Option<Exception>.None;
    }

    public async Task<Option<Exception>> DeleteByIdAsync(int chapterId)
    {
        var foundedChapter = await GetByIdAsync(chapterId);
        if (foundedChapter is null)
        {
            var entityNotFoundException = new EntityNotFoundException(nameof(ChapterEntityModel), $"Id={chapterId}");
            return Option<Exception>.Some(entityNotFoundException);
        }

        return await DeleteAsync(foundedChapter);
    }

    public async Task<Result<ChapterEntityModel>> UpdateAsync(ChapterEntityModel chapter)
    {
        try
        {
            var entryEntity = _applicationDbContext.Chapters.Entry(chapter);
            _applicationDbContext.Chapters.Update(entryEntity.Entity);
            await _applicationDbContext.SaveChangesAsync();
            return entryEntity.Entity;
        }
        catch (DbUpdateException)
        {
            var dbSavingException = new DbSavingException(nameof(ChapterEntityModel), DbSavingTypesEnum.Update);
            return new Result<ChapterEntityModel>(dbSavingException);
        }
    }

    public Result<ChapterEntityModel> UpdateBookAsync(int chapterId, BookEntityModel book)
    {
        throw new NotImplementedException();
    }

    public Result<ChapterEntityModel> UpdateTextAsync(int chapterId, string newText)
    {
        throw new NotImplementedException();
    }
}