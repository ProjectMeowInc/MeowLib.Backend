namespace MeowLib.Domain.Exceptions.Translation;

public class TranslationNotFoundException(int translationId)
    : ApiException($"Перевод с Id = {translationId} не найден");