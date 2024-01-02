namespace MeowLib.WebApi.Models.Responses.v1.People;

public class GetPeopleResponse
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required List<BookPeopleModel> Books { get; init; }
}