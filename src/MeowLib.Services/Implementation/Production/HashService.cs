using System.Security.Cryptography;
using System.Text;
using MeowLib.Services.Interface;

namespace MeowLib.Services.Implementation.Production;

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
        using var hashAlg = SHA512.Create();
        var hashedBytes = hashAlg.ComputeHash(stringBytes);
        return BitConverter.ToString(hashedBytes).Replace("-", string.Empty);
    }
}