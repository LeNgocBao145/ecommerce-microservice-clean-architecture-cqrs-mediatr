using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Commands.CreateOrder;
using OrderService.Application.DTOs;
using OrderService.Application.Queries.GetOrderById;
using System.Security.Claims;

namespace OrderService.WebAPI.Controllers
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class OrdersController(ISender mediator) : ControllerBase
    {
        [HttpGet]
        [Authorize(Policy = "UserOnly")]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrders()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }
            var orders = await mediator.Send(new GetOrdersByUserIdQuery(Guid.Parse(userId!)));
            return Ok(orders);
        }

        [HttpPost("checkout")]
        [Authorize(Policy = "UserOnly")]
        public async Task<ActionResult<OrderDTO>> CheckoutOrder([FromBody] CheckoutRequestDTO request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var checkoutCommand = new CheckoutOrderCommand(
                Guid.Parse(userId!),
                request.Notes,
                request.Coupon
            );

            var order = await mediator.Send(checkoutCommand);
            if (order == null)
            {
                return BadRequest("Failed to checkout.");
            }
            return Ok(order);
        }
    }
}