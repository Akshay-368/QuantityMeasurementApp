using Microsoft.AspNetCore.Mvc;
using QuantityMeasurement.Application.DTOs;
using QuantityMeasurement.Application.Interfaces;

namespace QuantityMeasurement.API.Controllers;
/// <summary>
/// This is a controller for the Quantity.
/// To expose the service to the outside world through HTTP
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class QuantityController : ControllerBase
{
    private readonly IQuantityService _quantityService;

    public QuantityController(IQuantityService quantityService)
    {
        _quantityService = quantityService;
    }

    [HttpPost("convert")]
    public IActionResult Convert([FromBody] QuantityRequestDto request)
    {
        var result = _quantityService.Convert(request);

        return Ok(result);
    }

    [HttpPost("add")]
    public IActionResult Add([FromBody] QuantityRequestDto request)
    {
        return Ok(_quantityService.Add(request));
    }

    [HttpPost("subtract")]
    public IActionResult Subtract([FromBody] QuantityRequestDto request)
    {
        return Ok(_quantityService.Subtract(request));
    }

    /// <summary>
    /// Divide the quantity by a scalar.
    /// </summary>
    /// <param name="request">The quantity and scalar to divide by.</param>
    /// <returns>The result of the division.</returns>
    [HttpPost("divide-scalar")]
    public IActionResult DivideScalar([FromBody] QuantityRequestDto request)
    {
        /// <summary>
        /// Divide the quantity by a scalar.
        /// </summary>
        /// <param name="request">The quantity and scalar to divide by.</param>
        /// <returns>The result of the division.</returns>
        return Ok(_quantityService.DivideByScalar(request));
    }

    [HttpPost("divide-quantity")]
    public IActionResult DivideQuantity([FromBody] QuantityRequestDto request)
    {
        return Ok(_quantityService.DivideByQuantity(request));
    }

}