using Mapster;
using OrderService.Application.DTOs;
using OrderService.Domain.Entities;

namespace OrderService.Application.Mappings
{
    public static class MapsterConfig
    {
        public static void Register()
        {
            TypeAdapterConfig<Order, OrderDTO>
                .NewConfig()
                .Map(dest => dest.OrderItems, src => src.OrderItems);

            TypeAdapterConfig<OrderItem, OrderItemDTO>
                .NewConfig();

            TypeAdapterConfig<Cart, CartDTO>
                .NewConfig()
                .Map(dest => dest.CartItems, src => src.CartItems);

            TypeAdapterConfig<CartItem, CartItemDTO>
                .NewConfig();
        }
    }
}
