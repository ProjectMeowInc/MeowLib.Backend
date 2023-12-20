namespace MeowLib.Domain.Exceptions.Tag;

public class TagNotFoundException(int tagId) : ApiException($"Тег с Id = {tagId} не найден");