using MapsterMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OrderService.Application.DTOs;
using OrderService.Domain.Entities;
using OrderService.Domain.Interfaces;
using OrderService.GrpcClients.Interfaces;

namespace OrderService.Application.Commands.CreateCardItem
{
    public class CreateCardItemCommandHandler(
        IUnitOfWork unitOfWork,
        IProductGrpcClient productGrpcClient,
        IMapper mapper,
        ILogger<CreateCardItemCommandHandler> logger) : IRequestHandler<CreateCardItemCommand, CartItemDTO>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IProductGrpcClient _productGrpcClient = productGrpcClient;
        private readonly ILogger<CreateCardItemCommandHandler> _logger = logger;

        public async Task<CartItemDTO> Handle(CreateCardItemCommand request, CancellationToken cancellationToken)
        {
            // Verify product stock before adding to cart
            //var stockResponse = await _productGrpcClient.GetProductStock(request.ProductId);

            //if (!stockResponse.Success || stockResponse.Stock < request.Quantity)
            //{
            //    throw new InvalidOperationException($"Insufficient stock for product. Available: {stockResponse.Stock}");
            //}

            // Map the command to a CartItem entity
            var cartItem = mapper.Map<CartItem>(request);

            await _unitOfWork.CartRepository.CreateCartItemAsync(cartItem);
            await _unitOfWork.SaveAsync(cancellationToken);

            return mapper.Map<CartItemDTO>(cartItem);
        }
    }
}
