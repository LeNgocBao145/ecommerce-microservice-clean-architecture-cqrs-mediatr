using MapsterMapper;
using MediatR;
using OrderService.Application.DTOs;
using OrderService.Domain.Interfaces;

namespace OrderService.Application.Queries.GetOrderById
{
    public class GetOrdersByUserIdQueryHandler : IRequestHandler<GetOrdersByUserIdQuery, IEnumerable<OrderDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetOrdersByUserIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IEnumerable<OrderDTO>> Handle(GetOrdersByUserIdQuery request, CancellationToken cancellationToken)
        {
            var orders = await _unitOfWork.OrderRepository.GetOrdersByAsync(o => o.UserId == request.UserId);
            return _mapper.Map<IEnumerable<OrderDTO>>(orders);
        }
    }
}