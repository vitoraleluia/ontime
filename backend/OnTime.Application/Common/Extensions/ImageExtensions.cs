using OnTime.Domain.Entities;

namespace OnTime.Application.Common.Extensions;

public static class ImageExtensions
{
    public static string? BuildImageUrl(this Image? image)
    {
        if (image == null || string.IsNullOrEmpty(image.ProcessedPath))
        {
            return null;
        }

        var cdnUrl = AppSettings.Configuration["ImageSettings:CdnUrl"];
        if (string.IsNullOrEmpty(cdnUrl))
        {
            return null;
        }

        return $"{cdnUrl.TrimEnd('/')}{image.ProcessedPath}";
    }
}