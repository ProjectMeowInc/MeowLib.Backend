namespace MeowLib.WebApi.Models.Responses.v1.Book;

public class BookShortModel
{
    public required int Id { get; init; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string? Image { get; set; }
}