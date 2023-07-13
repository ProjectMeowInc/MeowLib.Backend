namespace MeowLib.Domain.Dto.Book;

public class BookDto
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string? ImageName { get; set; }
}