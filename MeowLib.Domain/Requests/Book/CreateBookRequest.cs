namespace MeowLib.Domain.Requests.Book;

public class CreateBookRequest
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
}