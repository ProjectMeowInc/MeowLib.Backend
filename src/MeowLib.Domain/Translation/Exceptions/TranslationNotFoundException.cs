using MeowLib.Domain.Shared;

namespace MeowLib.Domain.Translation.Exceptions;

public class TranslationNotFoundException(int translationId)
    : ApiException($"Перевод с Id = {translationId} не найден");