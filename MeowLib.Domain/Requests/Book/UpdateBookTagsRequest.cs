namespace MeowLib.Domain.Requests.Book;

public class UpdateBookTagsRequest
{
    public required IEnumerable<int> Tags { get; set; }
}