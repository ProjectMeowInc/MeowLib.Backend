namespace MeowLib.Domain.File.Entity;

public class FileEntityModel
{
    public int Id { get; init; }
    public required string FileSystemName { get; init; }
    public required DateTime UploadAt { get; init; }
}