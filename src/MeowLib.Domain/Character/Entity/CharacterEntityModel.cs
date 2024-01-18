using MeowLib.Domain.File.Entity;

namespace MeowLib.Domain.Character.Entity;

public class CharacterEntityModel
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required FileEntityModel Image { get; init; }
}