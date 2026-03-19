using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sales.Orders.Application.Commands;
using Sales.Orders.Application.Queries;

namespace Sales.Orders.Api.Controllers;

[ApiController]
[Route("api/orders")]
public class OrderController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrderController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(result.Error);
        return Created($"/api/orders/{result.Value}", result.Value);
    }

    public record AddOrderItemRequest(Guid ProductId, string ProductName, double Quantity, decimal UnitPrice, decimal Discount);

    [HttpPost("{id}/items")]
    public async Task<IActionResult> AddItem(Guid id, [FromBody] AddOrderItemRequest request)
    {
        var command = new AddOrderItemCommand(id, request.ProductId, request.ProductName, request.Quantity, request.UnitPrice, request.Discount);
        var result = await _mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(result.Error);
        return Created($"/api/orders/{id}/items/{result.Value}", result.Value);
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPendingOrders()
    {
        var result = await _mediator.Send(new GetPendingOrdersQuery());
        if (!result.IsSuccess) return BadRequest(result.Error);
        return Ok(result.Value);
    }
}