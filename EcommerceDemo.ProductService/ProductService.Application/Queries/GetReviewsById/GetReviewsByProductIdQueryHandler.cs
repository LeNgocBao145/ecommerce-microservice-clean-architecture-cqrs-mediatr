using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductService.Application.Common;
using ProductService.Application.DTOs;
using ProductService.Domain.Exceptions;
using ProductService.Domain.Interfaces;

namespace ProductService.Application.Queries.GetReviewsById
{
    public class GetReviewsByProductIdQueryHandler(IReviewRepository reviewRepository,
        IMapper mapper) : IRequestHandler<GetReviewsByProductIdQuery, PagedResponse<ReviewDTO>>
    {
        public async Task<PagedResponse<ReviewDTO>> Handle(GetReviewsByProductIdQuery request, CancellationToken cancellationToken)
        {
            // 1. Lấy Queryable ban đầu (Chưa thực thi)
            var query = reviewRepository.GetQueryable().Where(r => r.ProductId == request.ProductId).AsNoTracking();

            // 4. Đếm tổng số bản ghi (Chỉ gọi 1 lần duy nhất)
            var totalRecords = await query.CountAsync(cancellationToken);

            // 5. Tính toán Metadata
            var totalPages = (int)Math.Ceiling(totalRecords / (double)request.PageSize);

            if (totalRecords == 0)
            {
                return new PagedResponse<ReviewDTO>
                {
                    Data = [],
                    Metadata = new PagedMetadata
                    {
                        AverageRating = null,
                        PageNumber = request.PageNumber,
                        PageSize = request.PageSize,
                        TotalRecords = 0,
                        TotalPages = 0
                    }
                };
            }

            var averageRating = await query.AverageAsync(r => r.Rating, cancellationToken);

            if (request.PageNumber > totalPages)
            {
                throw new InvalidParamsException("Invalid PageNumber param");
            }

            var paginationMetadata = new PagedMetadata
            {
                AverageRating = averageRating,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalRecords = totalRecords,
                TotalPages = totalPages
            };

            // 6. THỰC HIỆN PHÂN TRANG (Skip và Take)
            // Lưu ý: Luôn phải có OrderBy trước khi Skip/Take để kết quả ổn định
            var data = await query
                .OrderBy(r => r.Id) // Order by Id property
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ProjectTo<ReviewDTO>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return new PagedResponse<ReviewDTO>
            {
                Data = data,
                Metadata = paginationMetadata
            };
        }
    }
}
