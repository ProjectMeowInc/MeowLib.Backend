using MeowLib.Domain.Dto.User;

namespace MeowLib.Domain.Dto.BookComment;

public class BookCommentDto
{
    public required int Id { get; init; }
    public required string Text { get; init; }
    public required DateTime PostedAt { get; init; }
    public required UserDto Author { get; init; }
}