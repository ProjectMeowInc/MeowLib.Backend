using MeowLib.Domain.DbModels.BookEntity;
using MeowLib.Domain.DbModels.UserEntity;

namespace MeowLib.Domain.DbModels.BookCommentEntity;

/// <summary>
/// Класс, описывающий сущность комментария к книге.
/// </summary>
public class BookCommentEntityModel
{
    public int Id { get; set; }
    public required string Text { get; set; }
    public required DateTime PostedAt { get; set; }
    public required UserEntityModel Author { get; set; }
    public required BookEntityModel Book { get; set; }
}