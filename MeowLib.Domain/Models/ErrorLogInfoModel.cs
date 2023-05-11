namespace MeowLib.Domain.Models;

public class ErrorLogInfoModel
{
    public string ErrorModule { get; set; } = null!;
    public string Message { get; set; } = null!;
    public Dictionary<string, string>? AdditionalInfo { get; set; }
    public bool IsApiError { get; set; } = false;
}