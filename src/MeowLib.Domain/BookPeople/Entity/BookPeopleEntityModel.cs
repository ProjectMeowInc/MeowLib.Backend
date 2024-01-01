using MeowLib.Domain.Book.Entity;
using MeowLib.Domain.BookPeople.Enums;
using MeowLib.Domain.People.Entity;

namespace MeowLib.Domain.BookPeople.Entity;

public class BookPeopleEntityModel
{
    public int Id { get; init; }
    public required BookEntityModel Book { get; init; }
    public required PeopleEntityModel People { get; init; }
    public required BookPeopleRoleEnum Role { get; init; }
}