using System.Threading.Channels;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using OnTime.Application.Features.Images.Commands;
using OnTime.Application.Features.Images.Messages;
using OnTime.Application.Services;
using OnTime.Bus;
using OnTime.Domain.Entities;

namespace OnTime.Infrastructure.Consumers;

public class OptimizeImageConsumer : BusConsumer<OptimizeImageMessage>
{
    private readonly IServiceProvider serviceProvider;

    public OptimizeImageConsumer(
        Channel<OptimizeImageMessage> channel,
        IServiceProvider serviceProvider,
        ILogger<OptimizeImageConsumer> logger) : base(channel, logger)
    {
        this.serviceProvider = serviceProvider;
    }

    protected override async Task Consume(OptimizeImageMessage message)
    {
        this.Logger.LogInformation("Consuming OptimizeImageMessage for Image ID: {ImageId}", message.ImageId);

        using var scope = this.serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var result = await mediator.Send(new CreateOptimizedImageCommand(message.ImageId));
        if (result.IsFailure)
        {
            this.Logger.LogWarning("Failed to process Image ID {ImageId}: {Error}", message.ImageId,
                result.Error?.Message);
        }
    }

    protected override async Task OnStartedAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = this.serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

            // On start: queue images that are in pending state or we're left in processing state
            var unresolvedImages = await dbContext.Images
                .Where(i => i.Status == ImageStatus.Pending || i.Status == ImageStatus.Processing)
                .Select(i => i.Id)
                .ToListAsync(cancellationToken);

            var producer = scope.ServiceProvider.GetRequiredService<IBusProducer<OptimizeImageMessage>>();
            foreach (var imageId in unresolvedImages)
            {
                await producer.Publish(new OptimizeImageMessage(imageId), cancellationToken);
                this.Logger.LogInformation("Resumed unfinished image processing job for Image ID: {ImageId}", imageId);
            }
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Failed to load pending image jobs from database during startup.");
        }
    }
}