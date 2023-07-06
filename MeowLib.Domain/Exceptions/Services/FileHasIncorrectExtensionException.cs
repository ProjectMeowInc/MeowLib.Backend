namespace MeowLib.Domain.Exceptions.Services;

public class FileHasIncorrectExtensionException : ApiException
{
    public string CurrentException { get; protected set; }
    
    public FileHasIncorrectExtensionException(string errorMessage, string currentExtension) : base(errorMessage)
    {
        CurrentException = currentExtension;
    }
}