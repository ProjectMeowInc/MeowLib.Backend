namespace MeowLib.Domain.Requests.Tag;

public class CreateTagRequest
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}