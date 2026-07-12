using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnTime.Application.Features.Images.Commands;
using OnTime.Domain.Entities;

namespace OnTime.Site.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImagesController : ControllerBase
{
    private readonly IMediator mediator;

    public ImagesController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file, [FromQuery] ImageFormat format = ImageFormat.Square)
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
        var result = await this.mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(result.Error?.Message);
        }

        return Accepted(new { id = result.Value });
    }
}
