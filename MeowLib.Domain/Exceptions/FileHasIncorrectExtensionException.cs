namespace MeowLib.Domain.Exceptions;

/// <summary>
/// Класс исключений, когда файл имеет некорректное расширение.
/// </summary>
public class FileHasIncorrectExtensionException : ApiException
{
    /// <summary>
    /// Расширение, которое имел файл.
    /// </summary>
    public string CurrentExtension { get; protected set; }
    
    public FileHasIncorrectExtensionException(string errorMessage, string currentExtension) : base(errorMessage)
    {
        CurrentExtension = currentExtension;
    }
}