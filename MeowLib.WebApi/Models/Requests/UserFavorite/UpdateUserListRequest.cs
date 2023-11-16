using System.Text.Json.Serialization;
using MeowLib.Domain.Enums;

namespace MeowLib.WebApi.Models.Requests.UserFavorite;

public class UpdateUserListRequest
{
    [JsonPropertyName("bookId")]
    public int BookId { get; set; }

    [JsonPropertyName("status")]
    public UserFavoritesStatusEnum Status { get; set; }
}