using MediatR;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using OnTime.Application.Domain.Results;
using OnTime.Application.Domain.Settings;
using OnTime.Application.Features.Images.Messages;
using OnTime.Application.Services;
using OnTime.Bus;
using OnTime.Domain.Entities;

namespace OnTime.Application.Features.Images.Commands;

public record StoreImageCommand(
    Stream FileStream,
    string FileName,
    string ContentType,
    ImageFormat Format) : IRequest<Result<Guid>>;

public class StoreImageCommandHandler : BaseHandler<StoreImageCommand, Result<Guid>>
{
    private readonly IAppDbContext dbContext;
    private readonly IFileService fileService;
    private readonly IBusProducer<OptimizeImageMessage> producer;
    private readonly ImageStorageSettings storageSettings;

    public StoreImageCommandHandler(
        IAppDbContext dbContext,
        IFileService fileService,
        IBusProducer<OptimizeImageMessage> producer,
        IOptions<ImageStorageSettings> storageSettings,
        ILogger<StoreImageCommandHandler> logger) : base(logger)
    {
        this.dbContext = dbContext;
        this.fileService = fileService;
        this.producer = producer;
        this.storageSettings = storageSettings.Value;
    }

    protected override async Task<Result<Guid>> HandleSafe(StoreImageCommand request,
        CancellationToken cancellationToken)
    {
        var imageId = Guid.NewGuid();
        var extension = Path.GetExtension(request.FileName);

        var rawFolder = Path.IsPathRooted(this.storageSettings.RawFolder)
            ? this.storageSettings.RawFolder
            : Path.Combine(Directory.GetCurrentDirectory(), this.storageSettings.RawFolder);

        var rawFilePath = Path.Combine(rawFolder, $"{imageId}{extension}");

        // Save raw file using abstracted file service
        this.fileService.EnsureDirectoryExists(rawFilePath);
        await this.fileService.SaveFileAsync(rawFilePath, request.FileStream, cancellationToken);

        // Add record to DB
        var image = new Image
        {
            Id = imageId,
            SourceFileName = request.FileName,
            SourcePath = rawFilePath,
            SourceExtension = extension,
            ContentType = request.ContentType,
            Format = request.Format,
            Status = ImageStatus.Pending
        };

        this.dbContext.Images.Add(image);
        await this.dbContext.SaveChangesAsync(cancellationToken);

        await this.producer.Publish(new OptimizeImageMessage(imageId), cancellationToken);

        return Result<Guid>.Success(imageId);
    }
}