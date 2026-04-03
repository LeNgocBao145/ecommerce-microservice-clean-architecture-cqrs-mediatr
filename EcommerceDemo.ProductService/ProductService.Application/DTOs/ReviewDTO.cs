namespace ProductService.Application.DTOs
{
    public record ReviewDTO(
        Guid Id,
        Guid UserId,
        Guid ProductId,
        int Rating,
        string Comment,
        DateTime CreatedDate
    );
}
