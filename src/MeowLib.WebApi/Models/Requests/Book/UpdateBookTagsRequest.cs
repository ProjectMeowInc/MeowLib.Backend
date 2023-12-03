namespace MeowLib.WebApi.Models.Requests.Book;

public class UpdateBookTagsRequest
{
    public required IEnumerable<int> Tags { get; set; }
}