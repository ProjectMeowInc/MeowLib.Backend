namespace MeowLib.WebApi.Models.Responses.v1.People;

public class GetAllPeoplesResponse
{
    public required List<PeopleModel> Items { get; init; }
    public required int Page { get; init; }
}