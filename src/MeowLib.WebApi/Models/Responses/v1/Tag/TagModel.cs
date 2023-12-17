namespace MeowLib.WebApi.Models.Responses.v1.Tag;

public class TagModel
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
}