using Mapster;
using Microsoft.AspNetCore.Mvc;
using CleanApi.Api.Dtos;
using CleanApi.Core.Interfaces;
using CleanApi.Core.Models;

namespace CleanApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IOrderRepository _orderRepository;

    public OrdersController(IOrderService orderService, IOrderRepository orderRepository)
    {
        _orderService = orderService;
        _orderRepository = orderRepository;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto request)
    {
        var createModel = request.Adapt<CreateOrderModel>();

        try
        {
            var createdOrder = await _orderService.CreateOrderAsync(createModel);
            return CreatedAtAction(nameof(GetOrder), new { id = createdOrder.Id }, createdOrder);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(int id)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order == null) return NotFound();

        return Ok(order.Adapt<OrderSummaryResponseDto>());
    }

    [HttpGet("customer/{customerId}")]
    public async Task<IActionResult> GetOrdersByCustomer(int customerId)
    {
        var orders = await _orderRepository.GetOrdersByCustomerIdAsync(customerId);
        if (orders == null || !orders.Any()) return NotFound($"No orders found for customer ID {customerId}.");

        return Ok(orders.Adapt<IEnumerable<OrderSummaryResponseDto>>());
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto request)
    {
        var updateModel = new UpdateOrderStatusModel
        {
            OrderId = id,
            NewStatusName = request.NewStatusName
        };

        try
        {
            await _orderService.UpdateOrderStatusAsync(updateModel);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        try
        {
            await _orderService.DeleteOrderAsync(id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }
}