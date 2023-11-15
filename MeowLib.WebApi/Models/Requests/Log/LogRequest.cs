using MeowLib.Domain.Models;

namespace MeowLib.WebApi.Models.Requests.Log;

public class LogRequest
{
    public required ErrorLogInfoModel ErrorLog { get; set; }
}