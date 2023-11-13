using System.Text.Json.Serialization;

namespace MeowLib.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UserTeamMemberRoleEnum
{
    Standard = 1,
    Translator = 5,
    Redactor = 10,
    Admin = 1000
}