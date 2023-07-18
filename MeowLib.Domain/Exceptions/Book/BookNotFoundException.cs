namespace MeowLib.Domain.Exceptions.Book;

public class BookNotFoundException : ApiException
{
    public BookNotFoundException(int bookId) : base($"Книга с Id = {bookId} не найдена")
    {
    }
}