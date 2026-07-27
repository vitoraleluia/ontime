using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using OnTime.Application.Domain.Results;
using OnTime.Application.Extensions;
using OnTime.Application.Features.Shops.Responses;
using OnTime.Application.Services;
using OnTime.Domain.Common;

namespace OnTime.Application.Features.Shops.Queries;

public record GetShopBySlugQuery(string Slug) : IRequest<Result<ShopResponse>>;

public class GetShopBySlugQueryHandler : BaseHandler<GetShopBySlugQuery, Result<ShopResponse>>
{
    private readonly IAppDbContext dbContext;

    public GetShopBySlugQueryHandler(
        IAppDbContext dbContext,
        ILogger<GetShopBySlugQueryHandler> logger) : base(logger)
    {
        this.dbContext = dbContext;
    }

    protected override async Task<Result<ShopResponse>> HandleSafe(GetShopBySlugQuery request, CancellationToken cancellationToken)
    {
        var formattedSlug = request.Slug.BuildFriendlyUrl(50);

        var shop = await this.dbContext.Shops
            .Include(s => s.Image)
            .FirstOrDefaultAsync(s => s.Slug == formattedSlug || s.Slug == request.Slug, cancellationToken);

        if (shop == null)
        {
            return Result<ShopResponse>.Failure(new Error("ShopNotFound", "Estabelecimento não encontrado."));
        }

        var response = new ShopResponse
        {
            Id = shop.Id,
            OwnerId = shop.OwnerId,
            Name = shop.Name,
            Description = shop.Description,
            Slug = shop.Slug,
            Address = shop.Address,
            PhoneNumber = shop.PhoneNumber,
            SlotDurationMinutes = shop.SlotDurationMinutes,
            AllowCancellation = shop.AllowCancellation,
            CancellationDeadlineHours = shop.CancellationDeadlineHours,
            ImageId = shop.ImageId,
            ImageUrl = shop.Image.BuildImageUrl(),
            CreatedAt = shop.CreatedAt
        };

        return Result<ShopResponse>.Success(response);
    }
}
