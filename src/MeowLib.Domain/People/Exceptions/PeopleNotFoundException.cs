using MeowLib.Domain.Shared;

namespace MeowLib.Domain.People.Exceptions;

public class PeopleNotFoundException(int id) : ApiException($"Автор с Id = {id} не найден");