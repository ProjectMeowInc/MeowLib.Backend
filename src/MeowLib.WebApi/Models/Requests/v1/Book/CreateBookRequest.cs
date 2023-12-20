namespace MeowLib.WebApi.Models.Requests.v1.Book;

public class CreateBookRequest
{
    public required string Name { get; set; }
    public required string Description { get; set; }
}