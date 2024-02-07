namespace MeowLib.WebApi.Models.Responses.v1.People;

public class PeopleBookModel
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required string? Image { get; init; }
}