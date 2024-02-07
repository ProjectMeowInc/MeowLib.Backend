namespace MeowLib.WebApi.Models.Requests.v2.Book;

public class CreateBookRequest
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required int ImageId { get; init; }
}