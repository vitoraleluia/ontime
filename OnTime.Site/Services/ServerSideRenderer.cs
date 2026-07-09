using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

namespace OnTime.Site.Services;

public interface IServerSideRenderer
{
    Task<string> RenderView<TModel>(string viewName, TModel model);
}

public class ServerSideRenderer : IServerSideRenderer
{
    private readonly IRazorViewEngine razorViewEngine;
    private readonly ITempDataProvider tempDataProvider;
    private readonly IServiceProvider serviceProvider;

    public ServerSideRenderer(
        IRazorViewEngine razorViewEngine,
        ITempDataProvider tempDataProvider,
        IServiceProvider serviceProvider)
    {
        this.razorViewEngine = razorViewEngine;
        this.tempDataProvider = tempDataProvider;
        this.serviceProvider = serviceProvider;
    }

    public async Task<string> RenderView<TModel>(string viewName, TModel model)
    {
        var httpContext = new DefaultHttpContext { RequestServices = serviceProvider };
        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

        using var sw = new StringWriter();
        var viewResult = razorViewEngine.GetView(executingFilePath: null, viewPath: viewName, isMainPage: false);

        if (!viewResult.Success)
        {
            viewResult = razorViewEngine.FindView(actionContext, viewName, isMainPage: false);
        }

        if (!viewResult.Success)
        {
            throw new FileNotFoundException($"Não foi possível encontrar a vista '{viewName}'.");
        }

        var viewDictionary = new ViewDataDictionary<TModel>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
        {
            Model = model
        };

        var viewContext = new ViewContext(
            actionContext,
            viewResult.View,
            viewDictionary,
            new TempDataDictionary(actionContext.HttpContext, tempDataProvider),
            sw,
            new HtmlHelperOptions()
        );

        await viewResult.View.RenderAsync(viewContext);
        return sw.ToString();
    }
}
