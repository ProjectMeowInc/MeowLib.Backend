namespace MeowLib.Domain.Exceptions.Translation;

public class TeamAlreadyTranslateBookException(int teamId, int bookId) : ApiException(
    $"Комманда с Id = {teamId} уже занимается переводом книги с Id = {bookId}");