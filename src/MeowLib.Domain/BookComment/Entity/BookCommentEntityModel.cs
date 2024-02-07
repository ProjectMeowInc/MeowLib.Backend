using MeowLib.Domain.Book.Entity;
using MeowLib.Domain.User.Entity;

namespace MeowLib.Domain.BookComment.Entity;

/// <summary>
/// Класс, описывающий сущность комментария к книге.
/// </summary>
public class BookCommentEntityModel
{
    public int Id { get; init; }
    public required string Text { get; init; }
    public required DateTime PostedAt { get; init; }
    public required UserEntityModel Author { get; init; }
    public required BookEntityModel Book { get; init; }
}