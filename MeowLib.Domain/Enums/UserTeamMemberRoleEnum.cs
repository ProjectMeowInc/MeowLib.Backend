using System.Text.Json.Serialization;

namespace MeowLib.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UserTeamMemberRoleEnum
{
    Translator = 1,
    Redactor = 2,
    Admin = 1000
}