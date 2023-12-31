using System.ComponentModel;
using System.Text.Json.Serialization;

namespace MeowLib.Domain.User.Enums;

/// <summary>
/// Перечесление возможных ролей пользователя.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UserRolesEnum
{
    [Description("Обычный пользователь")]
    User = 1,

    [Description("Редактор")]
    Editor = 10,

    [Description("Модератор")]
    Moderator = 50,

    [Description("Администратор")]
    Admin = 999
}