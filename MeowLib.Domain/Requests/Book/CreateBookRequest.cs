namespace MeowLib.Domain.Requests.Book;

public class CreateBookRequest
{
    public required string Name { get; set; }
    public required string Description { get; set; }
}