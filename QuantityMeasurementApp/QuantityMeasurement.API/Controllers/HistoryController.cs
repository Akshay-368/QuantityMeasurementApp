using Microsoft.AspNetCore.Mvc;
using QuantityMeasurement.Infrastructure.Models;
using QuantityMeasurement.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
namespace QuantityMeasurement.API.Controllers;

[ApiController] // defines behaviour
[Route("api/[controller]")] // defines endpoint
[EnableRateLimiting("fixedWindowLimiter")]
[Authorize] // defines access rule  ( now everything here will require authentication) 
// Think of them in the order like : This is a controller -> at this route -> and it requires auth.
// and order does not matter , scope does matter .
// class level means applies to all of the endpoints and method level means overrides class
public class HistoryController : ControllerBase
{
    private readonly IHistoryRepository _historyRepository;

    public HistoryController(IHistoryRepository historyRepository)
    {
        _historyRepository = historyRepository;
    }

    [HttpGet]
    public ActionResult<List<HistoryRecord>> Get()
    {
        var history = _historyRepository.GetHistory();
        return Ok(history);
    }

    [HttpDelete]
    public IActionResult Delete()
    {
        _historyRepository.ClearHistory();
        return NoContent();
    }
}