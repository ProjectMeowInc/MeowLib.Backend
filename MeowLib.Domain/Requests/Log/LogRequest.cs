using MeowLib.Domain.Models;

namespace MeowLib.Domain.Requests.Log;

public class LogRequest
{
    public ErrorLogInfoModel ErrorLog { get; set; } = null!;
}