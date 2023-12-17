namespace MeowLib.WebApi.Models.Requests.v1.Tag;

public class CreateTagRequest
{
    public required string Name { get; set; }
    public required string? Description { get; set; }
}