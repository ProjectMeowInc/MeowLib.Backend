using MeowLib.Domain.Shared;

namespace MeowLib.Domain.Chapter.Exceptions;

public class ChapterNotFoundException : ApiException
{
    public ChapterNotFoundException(int chapterId) : base($"Глава с Id = {chapterId} не найдена") { }
}