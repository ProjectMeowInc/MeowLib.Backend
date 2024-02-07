using System.Text.Json.Serialization;

namespace MeowLib.Domain.BookPeople.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum BookPeopleRoleEnum
{
    Author = 1
}