using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using OnTime.Application.Features.Images.Commands;
using OnTime.Domain.Common;
using OnTime.Domain.Entities;
using OnTime.Domain.Enums;

namespace OnTime.Api.Controllers;

public record UploadImageResponse(Guid Id);

[Authorize]
[Produces("application/json")]
public class ImagesController : BaseApiController
{
    public ImagesController(ILogger<BaseApiController> logger, IMediator mediator) : base(logger, mediator)
    {
    }

    [HttpPost]
    [ProducesResponseType(typeof(UploadImageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UploadImageResponse>> Upload(IFormFile file,
        [FromQuery]
        ImageFormat format = ImageFormat.Square)
    {
        if (file == null || file.Length == 0)
        {
            return HandleFailure(ErrorCode.BadRequest, "Nenhum ficheiro enviado.");
        }

        if (!file.ContentType.StartsWith("image/"))
        {
            return HandleFailure(ErrorCode.BadRequest, "O ficheiro enviado não é uma imagem válida.");
        }

        using var stream = file.OpenReadStream();
        var command = new StoreImageCommand(stream, file.FileName, file.ContentType, format);
        var result = await Mediator.Send(command);

        if (result.IsFailure)
        {
            return HandleFailure(result.Error!);
        }

        return Ok(new UploadImageResponse(result.Value));
    }
}