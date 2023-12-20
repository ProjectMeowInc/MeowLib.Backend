namespace MeowLib.Domain.Exceptions.Services;

/// <summary>
/// Класс исключения возникающих при ошибке сохранения файла
/// </summary>
public class UploadingFileException : ApiException
{
    public UploadingFileException() : base("Ошибка загрузки файла") { }
}