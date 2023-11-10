using System.Text.Json.Serialization;
using MeowLib.Domain.Dto.BookComment;

namespace MeowLib.Domain.Responses.BookComment;

public class GetBookCommentsResponse
{
    [JsonPropertyName("bookId")] public required int BookId { get; set; }

    [JsonPropertyName("items")] public required IEnumerable<BookCommentDto> Items { get; set; }
}