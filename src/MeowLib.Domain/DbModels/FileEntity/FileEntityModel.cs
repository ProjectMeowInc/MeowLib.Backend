namespace MeowLib.Domain.DbModels.FileEntity;

public class FileEntityModel
{
    public int Id { get; init; }
    public required string FileSystemName { get; init; }
    public required DateTime UploadAt { get; init; }
}