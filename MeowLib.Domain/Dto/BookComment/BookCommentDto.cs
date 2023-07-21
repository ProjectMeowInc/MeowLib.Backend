using MeowLib.Domain.Dto.User;

namespace MeowLib.Domain.Dto.BookComment;

public class BookCommentDto
{
    public required int Id { get; set; }
    public required string Text { get; set; }
    public required DateTime PostedAt { get; set; }
    public required UserDto Author { get; set; }
}