using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using OnTime.Application.Services;

namespace OnTime.Infrastructure.Services;

public class ImageProcessingQueue : IImageProcessingQueue
{
    private readonly Channel<Guid> queue;

    public ImageProcessingQueue()
    {
        var options = new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false
        };
        this.queue = Channel.CreateUnbounded<Guid>(options);
    }

    public async ValueTask QueueBackgroundWorkItemAsync(Guid imageId)
    {
        await this.queue.Writer.WriteAsync(imageId);
    }

    public async ValueTask<Guid> DequeueAsync(CancellationToken cancellationToken)
    {
        return await this.queue.Reader.ReadAsync(cancellationToken);
    }
}
