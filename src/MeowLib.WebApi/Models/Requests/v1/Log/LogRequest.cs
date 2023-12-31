using MeowLib.Domain.Shared.Models;

namespace MeowLib.WebApi.Models.Requests.v1.Log;

public class LogRequest
{
    public required ErrorLogInfoModel ErrorLog { get; set; }
}