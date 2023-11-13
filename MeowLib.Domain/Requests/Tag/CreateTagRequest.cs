namespace MeowLib.Domain.Requests.Tag;

public class CreateTagRequest
{
    public required string Name { get; set; }
    public required string? Description { get; set; }
}