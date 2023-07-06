using System.Net.Http.Json;
using LanguageExt.Common;
using MeowLib.Domain.Exceptions;
using MeowLib.Domain.Exceptions.Services;
using MeowLib.Domain.ExternalRequests;
using MeowLib.Domain.ExternalResponses;
using MeowLIb.WebApi.Services.Interface;
using Microsoft.AspNetCore.Http;

namespace MeowLIb.WebApi.Services.Implementation.Production;

public class UploadFileService : IUploadFileService
{
    private readonly string _apiToken;
    private readonly HttpClient _httpClient;
    private static readonly string[] Validatedextension = { ".png", ".jpg" };

    public UploadFileService(string apiToken)
    {
        _apiToken = apiToken;
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri($"https://freeimage.host/api/1/upload"),
        };
    }

    public async Task<Result<string>> UploadImageAsync(IFormFile file)
    {
        var fileExtension = Path.GetExtension(file.FileName).ToLower();
        
        if (!Validatedextension.Contains(fileExtension))
        {
            var fileHasIncorrectExtensionException = new FileHasIncorrectExtensionException(
                "Файл имеет некорректное расширение", fileExtension);
            return new Result<string>(fileHasIncorrectExtensionException);
        }

        var base64UploadString = await ConvertToBase64Async(file);
        
        var uploadResult = await _httpClient.PostAsJsonAsync("", new UploadImageRequest
        {
            Key = _apiToken,
            Source = base64UploadString
        });
        
        var parsedContent = await uploadResult.Content.ReadFromJsonAsync<UploadImageResponse>();

        if (parsedContent is null)
        {
            var apiException = new ApiException("Ошибка парсинга ответа");
            return new Result<string>(apiException);
        }

        if (parsedContent.StatusCode != 200)
        {
            var apiException = new ApiException("Ошибка загрузки файла");
            return new Result<string>(apiException);
        }

        return parsedContent?.Url ?? "NO-LOADED";
    }

    private async Task<string> ConvertToBase64Async(IFormFile file)
    {
        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        var fileBytes = memoryStream.ToArray();
        return Convert.ToBase64String(fileBytes);
    }
}