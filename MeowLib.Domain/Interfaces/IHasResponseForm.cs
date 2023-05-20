
using Microsoft.AspNetCore.Mvc;

namespace MeowLib.Domain.Interfaces;

public interface IHasResponseForm 
{
    JsonResult ToResponse();
}