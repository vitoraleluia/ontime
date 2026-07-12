using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OnTime.Application.Features.Images.Commands;
using OnTime.Application.Services;
using OnTime.Domain.Entities;

namespace OnTime.Infrastructure.BackgroundServices;

public class ImageProcessingWorker : BackgroundService
{
    private readonly IImageProcessingQueue queue;
    private readonly IServiceProvider serviceProvider;
    private readonly ILogger<ImageProcessingWorker> logger;

    public ImageProcessingWorker(
        IImageProcessingQueue queue,
        IServiceProvider serviceProvider,
        ILogger<ImageProcessingWorker> logger)
    {
        this.queue = queue;
        this.serviceProvider = serviceProvider;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        this.logger.LogInformation("Image Processing Worker starting up...");

        // Resiliency: Scan database and enqueue any pending/processing jobs on startup
        try
        {
            using var scope = this.serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
            var unresolvedImages = await dbContext.Images
                .Where(i => i.Status == ImageStatus.Pending || i.Status == ImageStatus.Processing)
                .Select(i => i.Id)
                .ToListAsync(stoppingToken);

            foreach (var imageId in unresolvedImages)
            {
                await this.queue.QueueBackgroundWorkItemAsync(imageId);
                this.logger.LogInformation("Resumed unfinished image processing job for Image ID: {ImageId}", imageId);
            }
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Failed to load pending image jobs from database during startup.");
        }

        // Dispatch loop
        while (!stoppingToken.IsCancellationRequested)
        {
            Guid imageId = Guid.Empty;
            try
            {
                imageId = await this.queue.DequeueAsync(stoppingToken);
                this.logger.LogInformation("Dispatched processing job for Image ID: {ImageId}", imageId);

                using var scope = this.serviceProvider.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                var result = await mediator.Send(new CreateOptimizedImageCommand(imageId), stoppingToken);
                
                if (result.IsFailure)
                {
                    this.logger.LogWarning("Failed to process Image ID {ImageId}: {Error}", imageId, result.Error?.Message);
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Unexpected error dispatching processing job for Image ID: {ImageId}", imageId);
            }
        }
    }
}
