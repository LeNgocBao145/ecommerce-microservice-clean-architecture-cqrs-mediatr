using MapsterMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OrderService.Application.DTOs;
using OrderService.Domain.Entities;
using OrderService.Domain.Interfaces;

namespace OrderService.Application.Commands.CreateCart
{
    public class CreateCartCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<CreateCartCommandHandler> logger)
        : IRequestHandler<CreateCartCommand, CartDTO>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<CreateCartCommandHandler> _logger = logger;

        public async Task<CartDTO> Handle(CreateCartCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Creating new cart for user: {UserId}", request.UserId);

                // Create a new cart entity
                var cart = new Cart
                {
                    UserId = request.UserId,
                    CartItems = []
                };

                // Add the cart to the repository
                await _unitOfWork.CartRepository.CreateCartAsync(cart);

                // Save changes to the database
                await _unitOfWork.SaveAsync(cancellationToken);

                _logger.LogInformation("Cart created successfully with ID: {CartId} for user: {UserId}",
                    cart.Id, request.UserId);

                // Map the entity to DTO and return
                return mapper.Map<CartDTO>(cart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating cart for user: {UserId}", request.UserId);
                throw;
            }
        }
    }
}
