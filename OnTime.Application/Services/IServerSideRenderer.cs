using System.Threading.Tasks;

namespace OnTime.Application.Common.Interfaces;

public interface IServerSideRenderer
{
    Task<string> RenderView<TModel>(string viewName, TModel model);
}
