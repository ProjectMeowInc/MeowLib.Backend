using MeowLib.Domain.Book.Entity;
using MeowLib.Domain.Character.Enums;

namespace MeowLib.Domain.Character.Entity;

public class BookCharacterEntityModel
{
    public required int Id { get; init; }
    public required CharacterEntityModel Character { get; init; }
    public required BookEntityModel Book { get; init; }
    public required BookCharacterRoleEnum Role { get; init; }
}