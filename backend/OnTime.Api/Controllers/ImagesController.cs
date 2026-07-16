using MediatR;

using Microsoft.AspNetCore.Mvc;

using OnTime.Application.Features.Images.Commands;
using OnTime.Domain.Entities;
using OnTime.Site.Images;

using Microsoft.AspNetCore.Authorization;

namespace OnTime.Api.Controllers;

public record UploadImageResponse(Guid Id);

[Authorize]
public class ImagesController : BaseApiController
{
    public ImagesController(ILogger<BaseApiController> logger, IMediator mediator) : base(logger, mediator)
    {
    }

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file,
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

        return Accepted(new UploadImageResponse(result.Value));
    }
}