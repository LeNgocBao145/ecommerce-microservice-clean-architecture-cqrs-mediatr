using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Commands.CreateCardItem;
using OrderService.Application.Commands.CreateCart;
using OrderService.Application.Commands.UpdateCart;
using OrderService.Application.DTOs;
using OrderService.Application.Interfaces;
using OrderService.Application.Queries.GetCart;
using System.Security.Claims;

namespace OrderService.WebAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CartController(
        ISender mediator,
        IStockValidationService stockValidationService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<CartDTO>> GetCart()
        {
            // Logic to retrieve the cart
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }
            var cart = await mediator.Send(new GetCartQuery(Guid.Parse(userId)));
            return Ok(cart);
        }

        [HttpPost("add")]
        public async Task<ActionResult<CartItemDTO>> CreateCartItem(
            [FromBody] CreateCardItemCommand command,
            CancellationToken cancellationToken)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var cart = await mediator.Send(new GetCartQuery(Guid.Parse(userId)), cancellationToken);

            if (cart == null)
            {
                cart = await mediator.Send(new CreateCartCommand(Guid.Parse(userId)), cancellationToken);
                if (cart == null)
                {
                    return BadRequest("Failed to create cart.");
                }
            }

            try
            {
                await stockValidationService.ValidateStockAsync(
                    command.ProductId.ToString(),
                    command.Quantity,
                    cancellationToken);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

            command = command with { CartId = cart.Id };
            var cartItem = await mediator.Send(command, cancellationToken);

            return cartItem == null
                ? BadRequest("Failed to create cart item.")
                : Ok(cartItem);
        }

        [HttpPut("items/{cartItemId:guid}")]
        public async Task<ActionResult<CartItemDTO>> UpdateCartItem(
            Guid cartItemId,
            [FromBody] UpdateCartItemCommand command,
            CancellationToken cancellationToken)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var cart = await mediator.Send(new GetCartQuery(Guid.Parse(userId)), cancellationToken);

            if (cart == null)
            {
                return NotFound("Cart not found.");
            }

            if (!cart.CartItems.Any(ci => ci.Id == cartItemId))
            {
                return BadRequest("Cart item not found in cart.");
            }

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);
            if (cartItem != null && command.Quantity > 0)
            {
                try
                {
                    await stockValidationService.ValidateStockAsync(
                        cartItem.ProductId.ToString(),
                        command.Quantity,
                        cancellationToken);
                }
                catch (InvalidOperationException ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            command = command with { Id = cartItemId, CartId = cart.Id };
            var updatedCartItem = await mediator.Send(command, cancellationToken);
            return Ok(updatedCartItem);
        }
    }
}
