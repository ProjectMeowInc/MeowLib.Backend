namespace MeowLib.Domain.Character.Dto;

public class CharacterDto
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
}