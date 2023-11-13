namespace MeowLIb.Services.Interface;

/// <summary>
/// Абстракция сервиса для хеширования.
/// </summary>
public interface IHashService
{
    /// <summary>
    /// Метод хеширует строку и возвращает хешируемую строку.
    /// </summary>
    /// <param name="stringToHash"></param>
    /// <returns>Строка после хеширования</returns>
    string HashString(string stringToHash);
}