namespace OnTime.Application.Services;

public interface IServerSideRenderer
{
    Task<string> RenderView<TModel>(string viewName, TModel model);
}