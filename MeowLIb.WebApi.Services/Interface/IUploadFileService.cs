using LanguageExt.Common;
using Microsoft.AspNetCore.Http;

namespace MeowLIb.WebApi.Services.Interface;

public interface IUploadFileService
{
    Task<Result<string>> UploadBookImageAsync(IFormFile file);
    Task<(byte[]? content, string mimeType)> GetBookImageAsync(string imageName);
}