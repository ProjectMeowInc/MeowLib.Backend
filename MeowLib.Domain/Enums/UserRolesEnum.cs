using System.ComponentModel;
using System.Text.Json.Serialization;

namespace MeowLib.Domain.Enums;

/// <summary>
/// Перечесление возможных ролей пользователя.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UserRolesEnum
{
    [Description("Обычный пользователь")]
    User,
    [Description("Редактор")]
    Editor,
    [Description("Модератор")]
    Moderator = 50,
    [Description("Администратор")]
    Admin = 999
}