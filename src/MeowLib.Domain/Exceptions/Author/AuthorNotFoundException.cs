namespace MeowLib.Domain.Exceptions.Author;

public class AuthorNotFoundException(int id) : ApiException($"Автор с Id = {id} не найден");