namespace MeowLib.WebApi.Models.Responses.v1.Author;

public class GetAllAuthorsResponse
{
    public required IEnumerable<AuthorModel> Items { get; init; }
}