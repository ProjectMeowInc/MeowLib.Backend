using MeowLib.Domain.File.Entity;

namespace MeowLib.Domain.Character.Entity;

public class CharacterEntityModel
{
    public int Id { get; init; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required FileEntityModel Image { get; set; }
}