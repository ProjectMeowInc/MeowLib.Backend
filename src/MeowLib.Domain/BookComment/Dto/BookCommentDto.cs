using MeowLib.Domain.User.Dto;

namespace MeowLib.Domain.BookComment.Dto;

public class BookCommentDto
{
    public required int Id { get; init; }
    public required string Text { get; init; }
    public required DateTime PostedAt { get; init; }
    public required UserDto Author { get; init; }
}