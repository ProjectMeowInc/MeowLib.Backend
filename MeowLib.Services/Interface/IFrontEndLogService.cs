using MeowLib.Domain.Models;

namespace MeowLib.Services.Interface;

public interface IFrontEndLogService
{
    public Task LogAsync(string userLogin, ErrorLogInfoModel errorData);
}