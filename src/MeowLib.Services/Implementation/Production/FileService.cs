using MeowLib.DAL;
using MeowLib.Domain.File.Entity;
using MeowLib.Domain.File.Exceptions;
using MeowLib.Domain.File.Services;
using MeowLib.Domain.Shared.Result;
using Microsoft.AspNetCore.Http;

namespace MeowLib.Services.Implementation.Production;

/// <summary>
/// Сервис для работы с файлами
/// </summary>
public class FileService(ApplicationDbContext dbContext) : IFileService
{
    private const string UploadDirectoryPath = "./upload";
    private const long MaxFileSize = 512000; // 500 kb

    private static readonly string[] ValidateExtension = [".png", ".jpg"];

    private static readonly Dictionary<string, string> MimeMappings = new()
    {
        { "png", "image/png" },
        { "jpg", "image/jpg" }
    };


    public async Task<Result<FileEntityModel>> UploadFileAsync(IFormFile file)
    {
        if (file.Length > MaxFileSize)
        {
            return Result<FileEntityModel>.Fail(new FileIsTooBigException());
        }

        var fileExtension = Path.GetExtension(file.FileName).ToLower();

        if (!ValidateExtension.Contains(fileExtension))
        {
            var fileHasIncorrectExtensionException = new FileHasIncorrectExtensionException(
                "Файл имеет некорректное расширение", fileExtension);
            return Result<FileEntityModel>.Fail(fileHasIncorrectExtensionException);
        }

        var fileName = await SaveFileWithUniqueNameAsync(file, fileExtension);
        // todo: if save error need remove file
        var fileEntity = await dbContext.Files.AddAsync(new FileEntityModel
        {
            Id = 0,
            FileSystemName = fileName,
            UploadAt = DateTime.UtcNow
        });
        await dbContext.SaveChangesAsync();
        return fileEntity.Entity;
    }

    public async Task<(byte[] content, string mimeType)?> GetFileByNameAsync(string fileName)
    {
        if (fileName.Contains('/'))
        {
            return null;
        }

        var filePath = Path.Combine(UploadDirectoryPath, fileName);
        try
        {
            var fileData = await File.ReadAllBytesAsync(filePath);
            var fileExtension = Path.GetExtension(fileName);

            return (fileData, GetMimeTypeByExtension(fileExtension));
        }
        catch (FileNotFoundException)
        {
            return null;
        }
    }

    private async Task<string> SaveFileWithUniqueNameAsync(IFormFile file, string fileExtension)
    {
        var newFileName = $"{Guid.NewGuid()}.{fileExtension}";

        if (!Directory.Exists(UploadDirectoryPath))
        {
            Directory.CreateDirectory(UploadDirectoryPath);
        }

        await using var fileStream = File.Create(Path.Combine(UploadDirectoryPath, newFileName));
        await file.CopyToAsync(fileStream);

        return newFileName;
    }

    private string GetMimeTypeByExtension(string extension)
    {
        return MimeMappings.TryGetValue(extension, out var mime) ? mime : "application/unknown";
    }
}