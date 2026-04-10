using PromotionService.Application.DTOs.Requests;
using PromotionService.Application.Interfaces;

namespace PromotionService.WebAPI.Endpoints
{
    public static class CouponEndpoints
    {
        public static void MapCouponEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/v1")
                .WithTags("Coupon Management");

            // Admin endpoints
            var adminGroup = group.MapGroup("/admin/coupons")
                .WithTags("Admin");

            adminGroup.MapPost("/", CreateCoupon)
                .WithName("CreateCoupon")
                .WithDescription("Create a new coupon");

            // Public endpoints
            var cartGroup = group.MapGroup("/cart")
                .WithTags("Cart");

            cartGroup.MapPost("/apply-coupon", ApplyCoupon)
                .WithName("ApplyCoupon")
                .WithDescription("Apply a coupon to cart");
        }

        private static async Task<IResult> CreateCoupon(
            CreateCouponRequest request,
            ICouponService couponService)
        {
            try
            {
                var result = await couponService.CreateCouponAsync(request);
                return Results.CreatedAtRoute("CreateCoupon", result);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return Results.StatusCode(500);
            }
        }

        private static async Task<IResult> ApplyCoupon(
            ApplyCouponRequest request,
            ICouponService couponService)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Code) || request.OrderValue < 0)
                    return Results.BadRequest(new { message = "Invalid request parameters." });

                var result = await couponService.ApplyCouponAsync(request);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.InternalServerError(ex.Message);
            }
        }
    }
}