using MeowLib.Domain.BookPeople.Enums;

namespace MeowLib.WebApi.Models.Responses.v1.People;

public class BookPeopleModel
{
    public required PeopleBookModel Book { get; init; }
    public required BookPeopleRoleEnum Role { get; init; }
}