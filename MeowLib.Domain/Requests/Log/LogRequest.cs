using MeowLib.Domain.Models;

namespace MeowLib.Domain.Requests.Log;

public class LogRequest
{
    public required ErrorLogInfoModel ErrorLog { get; set; }
}