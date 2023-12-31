using MeowLib.Domain.File.Entity;
using MeowLib.Domain.File.Exceptions;
using MeowLib.Domain.Shared.Result;
using Microsoft.AspNetCore.Http;

namespace MeowLib.Domain.File.Services;

/// <summary>
/// Абстракция сервиса работы с файлами.
/// </summary>
public interface IFileService
{
    /// <summary>
    /// Метод загружает файл.
    /// </summary>
    /// <param name="file">Файл для загрузки.</param>
    /// <exception cref="FileHasIncorrectExtensionException">Возникает в случае, если переданный файл слишком большой.</exception>
    /// <exception cref="FileIsTooBigException">Возникает в случае, если файл имеет некорретное расширение.</exception>
    /// <returns>Созданный файл.</returns>
    Task<Result<FileEntityModel>> UploadFileAsync(IFormFile file);

    /// <summary>
    /// Метод получает файл по имени.
    /// </summary>
    /// <param name="fileName">Имя файла.</param>
    /// <returns>Полученный файл и подходящий Mime Type.</returns>
    Task<(byte[] content, string mimeType)?> GetFileByNameAsync(string fileName);
}