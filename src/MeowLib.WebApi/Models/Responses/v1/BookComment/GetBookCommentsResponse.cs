using System.Text.Json.Serialization;

namespace MeowLib.WebApi.Models.Responses.v1.BookComment;

public class GetBookCommentsResponse
{
    [JsonPropertyName("bookId")]
    public required int BookId { get; set; }

    [JsonPropertyName("items")]
    public required IEnumerable<BookCommentModel> Items { get; set; }
}