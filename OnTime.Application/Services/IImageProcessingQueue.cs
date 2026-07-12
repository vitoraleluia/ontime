using System;
using System.Threading;
using System.Threading.Tasks;

namespace OnTime.Application.Services;

public interface IImageProcessingQueue
{
    ValueTask QueueBackgroundWorkItemAsync(Guid imageId);
    ValueTask<Guid> DequeueAsync(CancellationToken cancellationToken);
}
