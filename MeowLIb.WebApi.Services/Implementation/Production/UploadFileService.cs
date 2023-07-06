using LanguageExt.Common;
using MeowLib.Domain.Exceptions.Services;
using MeowLIb.WebApi.Services.Interface;
using Microsoft.AspNetCore.Http;

namespace MeowLIb.WebApi.Services.Implementation.Production;

public class UploadFileService : IUploadFileService
{
    private readonly string _uploadDirectoryPath;
    private static readonly string[] ValidateExtension = { ".png", ".jpg" };
    private static readonly Dictionary<string, string> MimeMappings = new()
    {
        { "png", "image/png" },
        { "jpg", "image/jpg" }
    };

    public UploadFileService(string uploadDirectoryPath)
    {
        _uploadDirectoryPath = uploadDirectoryPath;
        if (!Directory.Exists(_uploadDirectoryPath))
        {
            Directory.CreateDirectory(_uploadDirectoryPath);
        }
    }

    public async Task<Result<string>> UploadBookImageAsync(IFormFile file)
    {
        var fileExtension = Path.GetExtension(file.FileName).ToLower();
        
        if (!ValidateExtension.Contains(fileExtension))
        {
            var fileHasIncorrectExtensionException = new FileHasIncorrectExtensionException(
                "Файл имеет некорректное расширение", fileExtension);
            return new Result<string>(fileHasIncorrectExtensionException);
        }

        return await SaveFileWithUniqueName(file, fileExtension, "book_photo");
    }

    public async Task<(byte[]? content, string mimeType)> GetBookImageAsync(string imageName)
    {
        var foundPath = Path.Combine(_uploadDirectoryPath, "book_photo", imageName);

        if (!File.Exists(foundPath))
        {
            return (null, "application/unknown");
        }

        var fileExtension = Path.GetExtension(foundPath);

        var fileContent = await File.ReadAllBytesAsync(foundPath);
        var mimeType = GetMimeTypeByExtension(fileExtension);
        return (fileContent, mimeType);
    }

    private async Task<string> SaveFileWithUniqueName(IFormFile file, string fileExtension, string filePrefix)
    {
        var newFileName = string.Concat(Path.GetRandomFileName().Replace(".", string.Empty), fileExtension);
        
        var uploadDirectory = Path.Combine(_uploadDirectoryPath, filePrefix);
        if (!Directory.Exists(uploadDirectory))
        {
            Directory.CreateDirectory(uploadDirectory);
        }
        
        await using var fileStream = File.Create($"{uploadDirectory}/{newFileName}");
        await file.CopyToAsync(fileStream);
        
        return newFileName;
    }

    private string GetMimeTypeByExtension(string extension)
    {
        return MimeMappings.TryGetValue(extension, out var mime) ? mime : "application/unknown";
    }
}