namespace MeowLib.WebApi.Models.Responses.v1.Tag;

public class GetAllTagsResponse
{
    public required IEnumerable<TagModel> Items { get; init; }
}