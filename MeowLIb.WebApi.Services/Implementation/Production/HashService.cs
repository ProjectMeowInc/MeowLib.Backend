using System.Security.Cryptography;
using System.Text;
using MeowLIb.WebApi.Services.Interface;

namespace MeowLIb.WebApi.Services.Implementation.Production;

/// <summary>
/// Сервис для хеширования.
/// </summary>
public class HashService : IHashService
{
    /// <summary>
    /// Метод хеширует строку и возвращает хешируемую строку.
    /// </summary>
    /// <param name="stringToHash"></param>
    /// <returns>Строка после хеширования</returns>
    public string HashString(string stringToHash)
    {
        var stringBytes = Encoding.UTF8.GetBytes(stringToHash);
        var hashedBytes = SHA256.HashData(stringBytes);
        return BitConverter.ToString(hashedBytes).Replace("-", string.Empty);
    }
}