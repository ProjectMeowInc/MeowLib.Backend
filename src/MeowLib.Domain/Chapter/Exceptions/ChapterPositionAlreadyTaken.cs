using MeowLib.Domain.Shared;

namespace MeowLib.Domain.Chapter.Exceptions;

public class ChapterPositionAlreadyTaken(uint position) : ApiException(
    $"Глава с заданной позицией ({position}) уже существует");