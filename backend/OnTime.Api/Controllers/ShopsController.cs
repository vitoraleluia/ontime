using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using OnTime.Api.Models.Shops;
using OnTime.Application.Domain.Constants;
using OnTime.Application.Features.Shops.Commands;
using OnTime.Application.Features.Shops.Queries;
using OnTime.Application.Features.Shops.Responses;
using OnTime.Domain.Common;
using OnTime.Domain.Enums;

namespace OnTime.Api.Controllers;

[Route("api/[controller]")]
public class ShopsController : BaseApiController
{
    public ShopsController(ILogger<BaseApiController> logger, IMediator mediator) : base(logger, mediator)
    {
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ShopResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ShopResponse>> CreateShop([FromBody] CreateShopRequest request)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                     ?? User.FindFirst(ClaimNames.Sub)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return HandleFailure(ErrorCode.Unauthorized, "ID de utilizador ausente no token.", StatusCodes.Status401Unauthorized);
        }

        if (!User.IsInRole(OnTime.Identity.Constants.IdentityRoles.Professional))
        {
            return HandleFailure(ErrorCode.ProfessionalRoleRequired, "Apenas contas com perfil profissional podem criar estabelecimentos.", StatusCodes.Status403Forbidden);
        }

        var command = new CreateShopCommand(
            userId,
            request.Name,
            request.Slug,
            request.Description,
            request.Address,
            request.PhoneNumber,
            request.SlotDurationMinutes,
            request.AllowCancellation,
            request.CancellationDeadlineHours,
            request.ImageId
        );

        var result = await this.Mediator.Send(command);

        if (result.IsFailure)
        {
            return HandleFailure(result.Error!);
        }

        return CreatedAtAction(nameof(GetShopBySlug), new { slug = result.Value!.Slug }, result.Value);
    }

    [HttpGet("check-slug")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(SlugAvailabilityResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SlugAvailabilityResponse>> CheckSlugAvailability([FromQuery] string slug)
    {
        var query = new CheckSlugAvailabilityQuery(slug);
        var result = await this.Mediator.Send(query);

        if (result.IsFailure)
        {
            return HandleFailure(result.Error!);
        }

        return Ok(result.Value!);
    }

    [HttpGet("{slug}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ShopResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ShopResponse>> GetShopBySlug(string slug)
    {
        var query = new GetShopBySlugQuery(slug);
        var result = await this.Mediator.Send(query);

        if (result.IsFailure)
        {
            return HandleFailure(result.Error!);
        }

        return Ok(result.Value!);
    }
}