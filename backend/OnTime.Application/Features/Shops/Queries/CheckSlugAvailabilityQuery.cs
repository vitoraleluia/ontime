using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using OnTime.Application.Domain.Results;
using OnTime.Application.Features.Shops.Responses;
using OnTime.Application.Services;
using OnTime.Domain.Common;

namespace OnTime.Application.Features.Shops.Queries;

public record CheckSlugAvailabilityQuery(string Slug) : IRequest<Result<SlugAvailabilityResponse>>;

public class CheckSlugAvailabilityQueryHandler : BaseHandler<CheckSlugAvailabilityQuery, Result<SlugAvailabilityResponse>>
{
    private readonly IAppDbContext dbContext;

    public CheckSlugAvailabilityQueryHandler(
        IAppDbContext dbContext,
        ILogger<CheckSlugAvailabilityQueryHandler> logger) : base(logger)
    {
        this.dbContext = dbContext;
    }

    protected override async Task<Result<SlugAvailabilityResponse>> HandleSafe(CheckSlugAvailabilityQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Slug))
        {
            return Result<SlugAvailabilityResponse>.Success(new SlugAvailabilityResponse
            {
                Slug = string.Empty,
                IsAvailable = false,
                Message = "O slug não pode estar vazio."
            });
        }

        var formattedSlug = request.Slug.BuildFriendlyUrl(50);

        if (string.IsNullOrWhiteSpace(formattedSlug))
        {
            return Result<SlugAvailabilityResponse>.Success(new SlugAvailabilityResponse
            {
                Slug = request.Slug,
                IsAvailable = false,
                Message = "O slug contém caracteres inválidos."
            });
        }

        var exists = await this.dbContext.Shops
            .AnyAsync(s => s.Slug == formattedSlug, cancellationToken);

        return Result<SlugAvailabilityResponse>.Success(new SlugAvailabilityResponse
        {
            Slug = formattedSlug,
            IsAvailable = !exists,
            Message = exists
                ? "Este slug já está em uso. Por favor escolha um slug diferente."
                : "Slug disponível!"
        });
    }
}
