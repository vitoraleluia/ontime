using System;
using System.Threading;
using System.Threading.Tasks;
using OnTime.Domain.Entities;

namespace OnTime.Application.Services;

public interface IImageProcessor
{
    Task ProcessImageAsync(Guid imageId, ImageFormat format, string sourcePath, CancellationToken cancellationToken);
}
