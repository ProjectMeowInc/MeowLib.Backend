namespace MeowLib.Domain.Shared.Models;

public class ErrorLogInfoModel
{
    public required string ErrorModule { get; init; }
    public required string Message { get; init; }
    public required Dictionary<string, string>? AdditionalInfo { get; init; }
    public required bool IsApiError { get; init; } = false;
}