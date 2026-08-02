namespace OnTime.Application.Services;

public interface IImageJobService
{
    Task OptimizeImage(Guid imageId);
}
