using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using OnTime.Application.Domain.Results;
using OnTime.Application.Extensions;
using OnTime.Application.Features.Shops.Queries;
using OnTime.Application.Features.Shops.Responses;
using OnTime.Application.Services;
using OnTime.Domain.Common;
using OnTime.Domain.Entities;

namespace OnTime.Application.Features.Shops.Commands;

public record CreateShopCommand(
    string OwnerId,
    string Name,
    string? Slug,
    string Description,
    string? Address,
    string? PhoneNumber,
    int SlotDurationMinutes,
    bool AllowCancellation,
    int CancellationDeadlineHours,
    Guid? ImageId
) : IRequest<Result<ShopResponse>>;

public class CreateShopCommandHandler : BaseHandler<CreateShopCommand, Result<ShopResponse>>
{
    private readonly IAppDbContext dbContext;
    private readonly IMediator mediator;

    public CreateShopCommandHandler(
        IAppDbContext dbContext,
        IMediator mediator,
        ILogger<CreateShopCommandHandler> logger) : base(logger)
    {
        this.dbContext = dbContext;
        this.mediator = mediator;
    }

    protected override async Task<Result<ShopResponse>> HandleSafe(CreateShopCommand request, CancellationToken cancellationToken)
    {
        var profile = await this.dbContext.UserProfiles
            .FirstOrDefaultAsync(u => u.Id == request.OwnerId, cancellationToken);

        if (profile == null)
        {
            return Result<ShopResponse>.Failure(new Error(
                "NotFound",
                "Perfil de utilizador não encontrado."));
        }

        var rawSlug = string.IsNullOrWhiteSpace(request.Slug) ? request.Name : request.Slug;
        var slugCheckResult = await this.mediator.Send(new CheckSlugAvailabilityQuery(rawSlug), cancellationToken);

        if (slugCheckResult.IsFailure)
        {
            return Result<ShopResponse>.Failure(slugCheckResult.Error!);
        }

        if (!slugCheckResult.Value!.IsAvailable)
        {
            var errorCode = string.IsNullOrEmpty(slugCheckResult.Value.Slug) ? "InvalidSlug" : "SlugAlreadyExists";
            return Result<ShopResponse>.Failure(new Error(errorCode, slugCheckResult.Value.Message));
        }

        var formattedSlug = slugCheckResult.Value.Slug;

        var shop = new Shop
        {
            OwnerId = request.OwnerId,
            Name = request.Name.Trim(),
            Description = request.Description?.Trim() ?? string.Empty,
            Slug = formattedSlug,
            Address = request.Address?.Trim(),
            PhoneNumber = request.PhoneNumber?.Trim(),
            SlotDurationMinutes = request.SlotDurationMinutes > 0 ? request.SlotDurationMinutes : 30,
            AllowCancellation = request.AllowCancellation,
            CancellationDeadlineHours = request.CancellationDeadlineHours >= 0 ? request.CancellationDeadlineHours : 24,
            ImageId = request.ImageId
        };

        this.dbContext.Shops.Add(shop);

        // Automatically register owner as an active ShopProfessional for their shop
        var shopProfessional = new ShopProfessional
        {
            Shop = shop,
            ProfessionalId = request.OwnerId,
            IsActive = true
        };

        this.dbContext.ShopProfessionals.Add(shopProfessional);

        await this.dbContext.SaveChangesAsync(cancellationToken);

        Image? loadedImage = null;
        if (shop.ImageId.HasValue)
        {
            loadedImage = await this.dbContext.Images.FirstOrDefaultAsync(i => i.Id == shop.ImageId.Value, cancellationToken);
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
            ImageUrl = loadedImage.BuildImageUrl(),
            CreatedAt = shop.CreatedAt
        };

        return Result<ShopResponse>.Success(response);
    }
}