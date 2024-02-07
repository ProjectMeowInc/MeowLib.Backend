using MeowLib.Domain.BookPeople.Enums;

namespace MeowLib.WebApi.Models.Responses.v2.Book;

public class PeopleWithBookRoleModel
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required BookPeopleRoleEnum Role { get; init; }
}