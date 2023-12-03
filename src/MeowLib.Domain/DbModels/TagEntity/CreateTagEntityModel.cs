namespace MeowLib.Domain.DbModels.TagEntity;

public class CreateTagEntityModel
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}