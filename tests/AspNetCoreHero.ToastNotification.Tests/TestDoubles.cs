using AspNetCoreHero.ToastNotification.Abstractions;
using AspNetCoreHero.ToastNotification.Containers;
using AspNetCoreHero.ToastNotification.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace AspNetCoreHero.ToastNotification.Tests;

/// <summary>Keeps TempData in memory, like the cookie provider would between two requests.</summary>
internal sealed class InMemoryTempDataProvider : ITempDataProvider
{
    private IDictionary<string, object> _values = new Dictionary<string, object>();

    public IDictionary<string, object> LoadTempData(HttpContext context) => new Dictionary<string, object>(_values);

    public void SaveTempData(HttpContext context, IDictionary<string, object> values) => _values = new Dictionary<string, object>(values);
}

internal sealed class SingleTempDataFactory(ITempDataDictionary tempData) : ITempDataDictionaryFactory
{
    public ITempDataDictionary GetTempData(HttpContext context) => tempData;
}

internal sealed class FixedContainerFactory(object container) : IMessageContainerFactory
{
    public IToastNotificationContainer<TMessage> Create<TMessage>() where TMessage : class
        => (IToastNotificationContainer<TMessage>)container;
}

internal static class TempDataFixture
{
    /// <summary>A real <see cref="TempDataService"/> on top of a real <see cref="TempDataDictionary"/>.</summary>
    public static (TempDataService Service, TempDataDictionary TempData, DefaultHttpContext HttpContext) Create(InMemoryTempDataProvider? provider = null)
    {
        var httpContext = new DefaultHttpContext();
        var tempData = new TempDataDictionary(httpContext, provider ?? new InMemoryTempDataProvider());
        var service = new TempDataService(new SingleTempDataFactory(tempData), new HttpContextAccessor { HttpContext = httpContext });
        return (service, tempData, httpContext);
    }
}
