using MeowLib.Domain.DbModels.BookEntity;
using MeowLib.Domain.DbModels.UserEntity;

namespace MeowLib.Domain.DbModels.BookCommentEntity;

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