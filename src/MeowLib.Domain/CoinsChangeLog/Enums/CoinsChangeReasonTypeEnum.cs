using System.Text.Json.Serialization;

namespace MeowLib.Domain.CoinsChangeLog.Enums;

/// <summary>
/// Причины изменения баланса.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CoinsChangeReasonTypeEnum
{
    /// <summary>
    /// Пополнение счёта пользователя администратором.
    /// </summary>
    AdminAddition = 1,

    /// <summary>
    /// Пополнение счёта пользователем.
    /// </summary>
    UserAddition = 2,

    /// <summary>
    /// Трата денег на покупку глав.
    /// </summary>
    BuyChapter = 3
}