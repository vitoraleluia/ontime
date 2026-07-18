using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using ImageMagick;

using Microsoft.Extensions.Options;

using OnTime.Application.Domain.Settings;
using OnTime.Application.Services;
using OnTime.Domain.Entities;

namespace OnTime.Infrastructure.Services;

public class ImageProcessor : IImageProcessor
{
    private readonly ImageSizeSettings sizeSettings;
    private readonly ImageStorageSettings storageSettings;

    public ImageProcessor(
        IOptions<ImageSizeSettings> sizeSettings,
        IOptions<ImageStorageSettings> storageSettings)
    {
        this.sizeSettings = sizeSettings.Value;
        this.storageSettings = storageSettings.Value;
    }

    public async Task ProcessImageAsync(Guid imageId, ImageFormat format, string sourcePath, CancellationToken cancellationToken)
    {
        if (!File.Exists(sourcePath))
        {
            throw new FileNotFoundException("Source file not found", sourcePath);
        }

        var yearMonth = DateTime.UtcNow.ToString("yyyy-MM");

        var processedPath = Path.IsPathRooted(this.storageSettings.ProcessedFolder)
            ? this.storageSettings.ProcessedFolder
            : Path.Combine(Directory.GetCurrentDirectory(), this.storageSettings.ProcessedFolder);

        var targetDir = Path.Combine(processedPath, yearMonth);
        Directory.CreateDirectory(targetDir);

        var outputPath = Path.Combine(targetDir, $"{imageId}.webp");

        uint width = 800;
        uint height = 800;

        switch (format)
        {
            case ImageFormat.Square:
                width = (uint)this.sizeSettings.Square.Width;
                height = (uint)this.sizeSettings.Square.Height;
                break;
            case ImageFormat.Landscape:
                width = (uint)this.sizeSettings.Landscape.Width;
                height = (uint)this.sizeSettings.Landscape.Height;
                break;
            case ImageFormat.Portrait:
                width = (uint)this.sizeSettings.Portrait.Width;
                height = (uint)this.sizeSettings.Portrait.Height;
                break;
        }

        // ImageMagick crop & resize
        using (var magickImage = new MagickImage(sourcePath))
        {
            var geometry = new MagickGeometry($"{width}x{height}^");
            magickImage.Resize(geometry);

            magickImage.Extent(width, height, Gravity.Center);

            magickImage.Format = MagickFormat.WebP;
            await magickImage.WriteAsync(outputPath, cancellationToken);
        }
    }
}