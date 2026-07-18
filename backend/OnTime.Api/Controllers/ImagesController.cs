using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using OnTime.Application.Features.Images.Commands;
using OnTime.Domain.Entities;
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
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UploadImageResponse>> Upload(IFormFile file,
        [FromQuery]
        ImageFormat format = ImageFormat.Square)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("Nenhum ficheiro enviado.");
        }

        if (!file.ContentType.StartsWith("image/"))
        {
            return BadRequest("O ficheiro enviado não é uma imagem válida.");
        }

        using var stream = file.OpenReadStream();
        var command = new StoreImageCommand(stream, file.FileName, file.ContentType, format);
        var result = await Mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(result.Error?.Message);
        }

        return Ok(new UploadImageResponse(result.Value));
    }
}