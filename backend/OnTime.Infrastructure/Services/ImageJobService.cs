using MediatR;

using Microsoft.Extensions.Logging;

using OnTime.Application.Features.Images.Commands;
using OnTime.Application.Services;

namespace OnTime.Infrastructure.Services;

public class ImageJobService : IImageJobService
{
    private readonly IMediator mediator;
    private readonly ILogger<ImageJobService> logger;

    public ImageJobService(IMediator mediator, ILogger<ImageJobService> logger)
    {
        this.mediator = mediator;
        this.logger = logger;
    }

    public async Task OptimizeImage(Guid imageId)
    {
        this.logger.LogInformation("Processing Hangfire image optimization job for Image ID: {ImageId}", imageId);
        var result = await this.mediator.Send(new CreateOptimizedImageCommand(imageId));
        if (result.IsFailure)
        {
            this.logger.LogWarning("Failed image optimization for Image ID {ImageId}: {Error}", imageId, result.Error?.Message);
        }
    }
}
