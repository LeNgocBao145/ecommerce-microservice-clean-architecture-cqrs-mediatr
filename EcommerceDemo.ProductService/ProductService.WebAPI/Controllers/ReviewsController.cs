using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Commands.CreateReview;
using ProductService.Application.DTOs;

namespace ProductService.WebAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ReviewsController(ISender mediator) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<ReviewDTO>> CreateReview([FromBody] CreateReviewCommand command)
        {
            // Implementation for creating a review will go here
            var review = await mediator.Send(command);
            if (review == null)
            {
                return BadRequest("Failed to create review.");
            }
            return Ok(review);
        }
    }
}
