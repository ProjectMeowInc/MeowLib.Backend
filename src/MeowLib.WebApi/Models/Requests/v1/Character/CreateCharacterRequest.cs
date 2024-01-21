namespace MeowLib.WebApi.Models.Requests.v1.Character;

public class CreateCharacterRequest
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required int ImageId { get; init; }
}