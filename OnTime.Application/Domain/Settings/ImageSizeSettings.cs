namespace OnTime.Application.Domain.Settings;

public class ImageSizeSettings
{
    public SizeSettings Square { get; set; } = new() { Width = 800, Height = 800 };
    public SizeSettings Landscape { get; set; } = new() { Width = 1200, Height = 675 };
    public SizeSettings Portrait { get; set; } = new() { Width = 800, Height = 1200 };
}

public class SizeSettings
{
    public int Width { get; set; }
    public int Height { get; set; }
}
