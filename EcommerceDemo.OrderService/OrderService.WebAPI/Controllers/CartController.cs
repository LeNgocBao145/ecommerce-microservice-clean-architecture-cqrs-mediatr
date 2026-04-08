using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Commands.CreateCardItem;
using OrderService.Application.Commands.CreateCart;
using OrderService.Application.Commands.UpdateCart;
using OrderService.Application.DTOs;
using OrderService.Application.Queries.GetCart;
using System.Security.Claims;

namespace OrderService.WebAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CartController(ISender mediator) : ControllerBase
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
        public async Task<ActionResult<CartItemDTO>> CreateCartItem([FromBody] CreateCardItemCommand command)
        {
            // Logic to create a new cart item
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var cart = await mediator.Send(new GetCartQuery(Guid.Parse(userId)));

            if (cart == null)
            {
                cart = await mediator.Send(new CreateCartCommand(Guid.Parse(userId)));

                if (cart == null)
                {
                    return BadRequest("Failed to create cart.");
                }
            }

            // gRPC call to ProductService to check stock availability

            command = command with { CartId = cart.Id };

            var cartItem = await mediator.Send(command);

            if (cartItem == null)
            {
                return BadRequest("Failed to create cart item.");
            }

            return Ok(cartItem);
        }

        [HttpPut("items/{cartItemId:guid}")]
        public async Task<ActionResult<CartItemDTO>> UpdateCartItem(Guid cartItemId, [FromBody] UpdateCartItemCommand command)
        {
            // Logic to update the cart
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var cart = await mediator.Send(new GetCartQuery(Guid.Parse(userId)));

            if (cart == null)
            {
                return NotFound("Cart not found.");
            }

            // Check if the cart item ID exists in the cart's items
            if (!cart.CartItems.Any(ci => ci.Id == cartItemId))
            {
                return BadRequest("Cart item not found in cart.");
            }

            command = command with { Id = cartItemId, CartId = cart.Id };
            var cartItem = await mediator.Send(command);
            return Ok(cartItem);
        }
        //[HttpDelete]
        //public Task<ActionResult> DeleteCart()
        //{
        //    // Logic to delete a cart
        //    return Ok();
        //}
    }
}
