using MeowLib.Domain.BookPeople.Enums;

namespace MeowLib.WebApi.Models.Requests.v1.Book;

public class AddPeopleRequest
{
    public required int PeopleId { get; init; }
    public required BookPeopleRoleEnum Role { get; init; }
}