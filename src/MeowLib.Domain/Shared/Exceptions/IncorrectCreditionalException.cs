namespace MeowLib.Domain.Shared.Exceptions;

/// <summary>
/// Класс описывающий исключения для некорректных авторизационные данных.
/// </summary>
public class IncorrectCreditionalException(string errorMessage) : Exception(errorMessage);