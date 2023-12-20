namespace MeowLib.Domain.Exceptions.Chapter;

public class ChapterPositionAlreadyTaken(uint position) : ApiException(
    $"Глава с заданной позицией ({position}) уже существует");