using MeowLib.DAL.Configuration;
using MeowLib.Domain.Book.Entity;
using MeowLib.Domain.BookComment.Entity;
using MeowLib.Domain.Bookmark.Entity;
using MeowLib.Domain.BookPeople.Entity;
using MeowLib.Domain.Chapter.Entity;
using MeowLib.Domain.Character.Entity;
using MeowLib.Domain.CoinsChangeLog.Entity;
using MeowLib.Domain.File.Entity;
using MeowLib.Domain.Notification.Entity;
using MeowLib.Domain.People.Entity;
using MeowLib.Domain.Tag.Entity;
using MeowLib.Domain.Team.Entity;
using MeowLib.Domain.TeamMember.Entity;
using MeowLib.Domain.Translation.Entity;
using MeowLib.Domain.User.Entity;
using MeowLib.Domain.UserFavorite.Entity;
using Microsoft.EntityFrameworkCore;

namespace MeowLib.DAL;

/// <summary>
/// Класс для работы EF.
/// </summary>
public class ApplicationDbContext : DbContext
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="options">Настройки БД.</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    /// <summary>
    /// Таблица пользователей.
    /// </summary>
    public required DbSet<UserEntityModel> Users { get; init; }

    /// <summary>
    /// Таблица авторов.
    /// </summary>
    public required DbSet<PeopleEntityModel> Peoples { get; init; }

    /// <summary>
    /// Таблица тегов.
    /// </summary>
    public required DbSet<TagEntityModel> Tags { get; init; }

    /// <summary>
    /// Таблица книг.
    /// </summary>
    public required DbSet<BookEntityModel> Books { get; init; }

    /// <summary>
    /// Таблица глав книг.
    /// </summary>
    public required DbSet<ChapterEntityModel> Chapters { get; init; }

    /// <summary>
    /// Таблица избранных книг пользователя.
    /// </summary>
    public required DbSet<UserFavoriteEntityModel> UsersFavorite { get; init; }

    /// <summary>
    /// Таблица закладок пользователей.
    /// </summary>
    public required DbSet<BookmarkEntityModel> Bookmarks { get; init; }

    /// <summary>
    /// Таблциа комментариев к книге.
    /// </summary>
    public required DbSet<BookCommentEntityModel> BookComments { get; init; }

    /// <summary>
    /// Таблица команд.
    /// </summary>
    public required DbSet<TeamEntityModel> Teams { get; init; }

    /// <summary>
    /// Таблица членов команд.
    /// </summary>
    public required DbSet<TeamMemberEntityModel> TeamMembers { get; init; }

    /// <summary>
    /// Таблица уведомлений пользователей.
    /// </summary>
    public required DbSet<NotificationEntityModel> Notifications { get; init; }

    /// <summary>
    /// Таблица переводов.
    /// </summary>
    public required DbSet<TranslationEntityModel> Translations { get; init; }

    /// <summary>
    /// Таблица логов измененния денег пользователя.
    /// </summary>
    public required DbSet<CoinsChangeLogEntityModel> CoinsChangeLog { get; init; }

    /// <summary>
    /// Таблица файлов.
    /// </summary>
    public required DbSet<FileEntityModel> Files { get; init; }

    /// <summary>
    /// Таблица для связи людей и книг.
    /// </summary>
    public required DbSet<BookPeopleEntityModel> BookPeople { get; init; }

    /// <summary>
    /// Таблица персонажей.
    /// </summary>
    public required DbSet<CharacterEntityModel> Characters { get; init; }

    /// <summary>
    /// Таблица для связи персонажей и книг.
    /// </summary>
    public required DbSet<BookCharacterEntityModel> BookCharacter { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new BookEntityModelConfiguration());

        modelBuilder.ApplyConfiguration(new TranslationEntityModelConfiguration());

        modelBuilder.ApplyConfiguration(new ChapterEntityModelConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}