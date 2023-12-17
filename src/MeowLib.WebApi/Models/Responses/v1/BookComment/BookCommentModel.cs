using MeowLib.Domain.Dto.User;

namespace MeowLib.WebApi.Models.Responses.v1.BookComment;

public class BookCommentModel
{
    public required int Id { get; init; }
    public required string Text { get; init; }
    public required DateTime PostedAt { get; init; }
    public required UserDto Author { get; init; }
}