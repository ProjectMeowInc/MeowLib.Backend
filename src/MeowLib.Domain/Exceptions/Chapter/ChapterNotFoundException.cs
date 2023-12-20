namespace MeowLib.Domain.Exceptions.Chapter;

public class ChapterNotFoundException : ApiException
{
    public ChapterNotFoundException(int chapterId) : base($"Глава с Id = {chapterId} не найдена") { }
}