using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using OnTime.Api.Models.Shops;
using OnTime.Application.Domain.Constants;
using OnTime.Application.Features.Shops.Commands;
using OnTime.Application.Features.Shops.Queries;
using OnTime.Application.Features.Shops.Responses;

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
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ShopResponse>> CreateShop([FromBody] CreateShopRequest request)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                     ?? User.FindFirst(ClaimNames.Sub)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("ID de utilizador ausente no token.");
        }

        if (!User.IsInRole(OnTime.Identity.Constants.IdentityRoles.Professional))
        {
            return StatusCode(StatusCodes.Status403Forbidden, "Apenas contas com perfil profissional podem criar estabelecimentos.");
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
            if (result.Error?.Code == "Forbidden")
            {
                return StatusCode(StatusCodes.Status403Forbidden, result.Error.Message);
            }

            return BadRequest(result.Error?.Message);
        }

        return CreatedAtAction(nameof(GetShopBySlug), new { slug = result.Value!.Slug }, result.Value);
    }

    [HttpGet("check-slug")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(SlugAvailabilityResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<SlugAvailabilityResponse>> CheckSlugAvailability([FromQuery] string slug)
    {
        var query = new CheckSlugAvailabilityQuery(slug);
        var result = await this.Mediator.Send(query);

        if (result.IsFailure)
        {
            return BadRequest(result.Error?.Message);
        }

        return Ok(result.Value!);
    }

    [HttpGet("{slug}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ShopResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ShopResponse>> GetShopBySlug(string slug)
    {
        var query = new GetShopBySlugQuery(slug);
        var result = await this.Mediator.Send(query);

        if (result.IsFailure)
        {
            return NotFound(result.Error?.Message);
        }

        return Ok(result.Value!);
    }
}
