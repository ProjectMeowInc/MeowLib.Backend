using MeowLib.Domain.Shared;

namespace MeowLib.Domain.File.Exceptions;

/// <summary>
/// Класс исключений, когда файл имеет некорректное расширение.
/// </summary>
public class FileHasIncorrectExtensionException(string errorMessage, string currentExtension)
    : ApiException(errorMessage)
{
    /// <summary>
    /// Расширение, которое имел файл.
    /// </summary>
    public string CurrentExtension { get; protected init; } = currentExtension;
}