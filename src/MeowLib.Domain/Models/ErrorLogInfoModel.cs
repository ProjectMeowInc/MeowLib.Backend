namespace MeowLib.Domain.Models;

public class ErrorLogInfoModel
{
    public required string ErrorModule { get; set; }
    public required string Message { get; set; }
    public required Dictionary<string, string>? AdditionalInfo { get; set; }
    public required bool IsApiError { get; set; } = false;
}