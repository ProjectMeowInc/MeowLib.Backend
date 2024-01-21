using System.Text.Json.Serialization;

namespace MeowLib.Domain.Character.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum BookCharacterRoleEnum
{
    MainCharacter = 1,
    Character = 2
}