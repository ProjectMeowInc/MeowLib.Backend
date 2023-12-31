using MeowLib.Domain.Shared;

namespace MeowLib.Domain.Tag.Exceptions;

public class TagNotFoundException(int tagId) : ApiException($"Тег с Id = {tagId} не найден");