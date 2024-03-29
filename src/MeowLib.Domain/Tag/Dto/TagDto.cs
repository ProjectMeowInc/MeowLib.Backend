namespace MeowLib.Domain.Tag.Dto;

public class TagDto
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
}