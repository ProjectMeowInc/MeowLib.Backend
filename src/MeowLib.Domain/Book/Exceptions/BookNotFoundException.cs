using MeowLib.Domain.Shared;

namespace MeowLib.Domain.Book.Exceptions;

public class BookNotFoundException : ApiException
{
    public BookNotFoundException(int bookId) : base($"Книга с Id = {bookId} не найдена") { }
}