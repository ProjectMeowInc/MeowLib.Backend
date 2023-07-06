using LanguageExt.Common;
using Microsoft.AspNetCore.Http;

namespace MeowLIb.WebApi.Services.Interface;

public interface IUploadFileService
{
    Task<Result<string>> UploadImageAsync(IFormFile file);
}