using MeowLib.Domain.Shared;

namespace MeowLib.Domain.Chapter.Exceptions;

public class ChapterPositionAlreadyTakenException(uint position) : ApiException(
    $"Глава с заданной позицией ({position}) уже существует");