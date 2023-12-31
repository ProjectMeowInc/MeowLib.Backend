using System.Text.Json.Serialization;

namespace MeowLib.Domain.UserFavorite.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UserFavoritesStatusEnum
{
    InPlans = 1,
    ReadingNow = 2,
    Favourite = 3,
    Read = 4
}