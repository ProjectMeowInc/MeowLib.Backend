
using Microsoft.AspNetCore.Mvc;

namespace MeowLib.Domain.Interfaces;

/// <summary>
/// Интерфейс для данных, которые можно привести к корректному виду для возвращения пользователю.
/// </summary>
public interface IHasResponseForm 
{
    /// <summary>
    /// Метод приводит к формату, пригодному для возвращения из контроллера.
    /// </summary>
    /// <returns>JsonResult, который можно вернуть пользователю.</returns>
    JsonResult ToResponse();
}