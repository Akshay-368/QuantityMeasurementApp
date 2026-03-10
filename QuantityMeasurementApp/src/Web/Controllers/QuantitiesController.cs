using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuantityMeasurement.Application.DTOs;
using QuantityMeasurement.Application.Features.Quantities;

namespace QuantityMeasurement.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuantitiesController : ControllerBase
{
    private readonly IMediator _mediator;

    public QuantitiesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // POST /api/quantities/add
    [HttpPost("add")]
    public async Task<ActionResult<ResultDto>> Add(AddQuantitiesCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    // POST /api/quantities/subtract
    [HttpPost("subtract")]
    public async Task<ActionResult<ResultDto>> Subtract(SubtractQuantitiesCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    // POST /api/quantities/convert
    [HttpPost("convert")]
    public async Task<ActionResult<ResultDto>> Convert(ConvertQuantityQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    // POST /api/quantities/compare
    [HttpPost("compare")]
    public async Task<ActionResult<bool>> Compare(CompareQuantitiesQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    // POST /api/quantities/divide
    [HttpPost("divide")]
    public async Task<ActionResult<double>> Divide(DivideQuantitiesQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    // POST /api/quantities/divide-scalar
    [HttpPost("divide-scalar")]
    public async Task<ActionResult<ResultDto>> DivideByScalar(DivideByScalarCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}