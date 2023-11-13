using MeowLib.Domain.Exceptions;
using MeowLib.Domain.Result;
using Microsoft.AspNetCore.Http;

namespace MeowLIb.Services.Interface;

/// <summary>
/// Абстракция сервиса работы с файлами.
/// </summary>
public interface IFileService
{
    /// <summary>
    /// Метод загружает изображение как изображение книги.
    /// </summary>
    /// <param name="file">Файл для загрузки.</param>
    /// <returns>Название созданного файла</returns>
    /// <exception cref="FileHasIncorrectExtensionException">Возникает в случае, если файл имеет некорретное расширение.</exception>
    Task<Result<string>> UploadBookImageAsync(IFormFile file);

    /// <summary>
    /// Метод возвращает загруженное ранее изображение книги. 
    /// </summary>
    /// <param name="imageName">Название книги.</param>
    /// <returns>Картинка, в виде байтов и её mimeType, если картинка не найдена, то content - null</returns>
    Task<(byte[]? content, string mimeType)> GetBookImageAsync(string imageName);
}