using MeowLib.Domain.DbModels.BookEntity;

namespace MeowLib.Domain.Responses.Book;

public class GetAllBooksResponse
{
    public IEnumerable<BookEntityModel> Items { get; init; } = null!;
}