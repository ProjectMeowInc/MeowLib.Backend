using MeowLib.DAL;
using MeowLib.Domain.Book.Exceptions;
using MeowLib.Domain.Book.Services;
using MeowLib.Domain.Character.Entity;
using MeowLib.Domain.Character.Enums;
using MeowLib.Domain.Character.Exceptions;
using MeowLib.Domain.Character.Services;
using MeowLib.Domain.Shared.Result;
using Microsoft.EntityFrameworkCore;

namespace MeowLib.Services.Implementation.Production;

public class BookCharacterService(
    ApplicationDbContext dbContext,
    ICharacterService characterService,
    IBookService bookService) : IBookCharacterService
{
    public async Task<Result<BookCharacterEntityModel>> AttachCharacterToBookAsync(int characterId, int bookId,
        BookCharacterRoleEnum role)
    {
        if (await CheckCharacterAlreadyAttachedAsync(characterId, bookId))
        {
            return Result<BookCharacterEntityModel>.Fail(new CharacterAlreadyAttachedToBookException());
        }

        var foundedCharacter = await characterService.GetCharacterByIdAsync(characterId);
        if (foundedCharacter is null)
        {
            return Result<BookCharacterEntityModel>.Fail(new CharacterNotFoundException());
        }

        var foundedBook = await bookService.GetBookByIdAsync(bookId);
        if (foundedBook is null)
        {
            return Result<BookCharacterEntityModel>.Fail(new BookNotFoundException(bookId));
        }

        var entry = await dbContext.BookCharacter.AddAsync(new BookCharacterEntityModel
        {
            Character = foundedCharacter,
            Book = foundedBook,
            Role = role
        });
        await dbContext.SaveChangesAsync();
        return entry.Entity;
    }

    public async Task<Result> UpdateBookCharacterRoleAsync(int characterId, int bookId, BookCharacterRoleEnum newRole)
    {
        var foundedBookCharacter = await dbContext.BookCharacter
            .FirstOrDefaultAsync(bc => bc.Character.Id == characterId
                                       && bc.Book.Id == bookId);

        if (foundedBookCharacter is null)
        {
            return Result.Fail(new BookCharacterNotFoundException());
        }

        foundedBookCharacter.Role = newRole;
        dbContext.BookCharacter.Update(foundedBookCharacter);
        await dbContext.SaveChangesAsync();

        return Result.Ok();
    }

    public async Task<Result> RemoveBookCharacterAsync(int characterId, int bookId)
    {
        var foundedBookCharacter =
            await dbContext.BookCharacter.FirstOrDefaultAsync(bc =>
                bc.Character.Id == characterId && bc.Book.Id == bookId);

        if (foundedBookCharacter is null)
        {
            return Result.Fail(new BookCharacterNotFoundException());
        }

        dbContext.BookCharacter.Remove(foundedBookCharacter);
        await dbContext.SaveChangesAsync();

        return Result.Ok();
    }

    public async Task<bool> CheckCharacterAlreadyAttachedAsync(int characterId, int bookId)
    {
        return await dbContext.BookCharacter.AnyAsync(bc => bc.Book.Id == bookId
                                                            && bc.Character.Id == characterId);
    }
}