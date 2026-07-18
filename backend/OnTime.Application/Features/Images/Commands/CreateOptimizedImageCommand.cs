using System;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Microsoft.Extensions.Logging;

using OnTime.Application.Domain.Results;
using OnTime.Application.Services;
using OnTime.Domain.Entities;

namespace OnTime.Application.Features.Images.Commands;

public record CreateOptimizedImageCommand(Guid ImageId) : IRequest<Result>;

public class CreateOptimizedImageCommandHandler : BaseHandler<CreateOptimizedImageCommand, Result>
{
    private readonly IAppDbContext dbContext;
    private readonly IImageProcessor imageProcessor;
    private readonly IFileService fileService;
    private readonly ILogger<CreateOptimizedImageCommandHandler> logger;

    public CreateOptimizedImageCommandHandler(
        IAppDbContext dbContext,
        IImageProcessor imageProcessor,
        IFileService fileService,
        ILogger<CreateOptimizedImageCommandHandler> logger) : base(logger)
    {
        this.dbContext = dbContext;
        this.imageProcessor = imageProcessor;
        this.fileService = fileService;
        this.logger = logger;
    }

    protected override async Task<Result> HandleSafe(CreateOptimizedImageCommand request, CancellationToken cancellationToken)
    {
        var image = await this.dbContext.Images.FindAsync(new object[] { request.ImageId }, cancellationToken);
        if (image == null)
        {
            return Result.Failure(new Error("ImageNotFound", $"Image {request.ImageId} not found."));
        }

        try
        {
            image.Status = ImageStatus.Processing;
            await this.dbContext.SaveChangesAsync(cancellationToken);

            // Execute file processing via Infrastructure processor
            await this.imageProcessor.ProcessImageAsync(image.Id, image.Format, image.SourcePath, cancellationToken);

            var yearMonth = DateTime.UtcNow.ToString("yyyy-MM");
            image.ProcessedPath = $"/{yearMonth}/{image.Id}.webp";
            image.Status = ImageStatus.Processed;

            await this.dbContext.SaveChangesAsync(cancellationToken);

            // Clean up original raw file using file service
            try
            {
                if (this.fileService.FileExists(image.SourcePath))
                {
                    this.fileService.DeleteFile(image.SourcePath);
                }
            }
            catch (Exception ex)
            {
                this.logger.LogWarning(ex, "Could not delete raw image file at {Path}", image.SourcePath);
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            image.Status = ImageStatus.Failed;
            image.ErrorMessage = ex.Message;
            await this.dbContext.SaveChangesAsync(cancellationToken);

            return Result.Failure(new Error("ImageProcessingFailed", ex.Message));
        }
    }
}