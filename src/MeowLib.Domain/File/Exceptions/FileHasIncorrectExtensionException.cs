namespace MeowLib.Domain.Exceptions.File;

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