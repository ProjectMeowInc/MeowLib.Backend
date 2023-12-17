namespace MeowLib.WebApi.Models.Requests.v1.Book;

public class UpdateBookTagsRequest
{
    public required IEnumerable<int> Tags { get; set; }
}