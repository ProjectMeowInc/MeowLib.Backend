using System.Text.Json.Serialization;
using MeowLib.Domain.UserFavorite.Enums;

namespace MeowLib.WebApi.Models.Requests.v1.UserFavorite;

public class UpdateUserListRequest
{
    [JsonPropertyName("bookId")]
    public int BookId { get; set; }

    [JsonPropertyName("status")]
    public UserFavoritesStatusEnum Status { get; set; }
}