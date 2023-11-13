using MeowLib.Domain.Models;

namespace MeowLIb.Services.Interface;

public interface IFrontEndLogService
{
    public Task LogAsync(string userLogin, ErrorLogInfoModel errorData);
}