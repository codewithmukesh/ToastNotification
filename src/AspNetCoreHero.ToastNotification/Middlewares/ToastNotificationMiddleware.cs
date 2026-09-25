using AspNetCoreHero.ToastNotification.Abstractions;
using AspNetCoreHero.ToastNotification.Containers;
using AspNetCoreHero.ToastNotification.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace AspNetCoreHero.ToastNotification.Middlewares
{
    /// <summary>
    /// For AJAX / fetch / htmx requests, writes the notifications raised during the request into a response header
    /// so the client script can show them without a page reload.
    /// </summary>
    internal sealed class ToastNotificationMiddleware
    {
        /// <summary>Stay well under common proxy header limits (nginx defaults to 4-8 KB for all headers).</summary>
        internal const int MaxHeaderLength = 4096;

        private const string AccessControlExposeHeadersKey = "Access-Control-Expose-Headers";
        private static readonly object RegisteredKey = new object();

        // The header value is URL-encoded and read with JSON.parse - never put into HTML - so relaxed escaping is safe
        // and keeps HTML messages from growing 5x.
        private static readonly JsonSerializerOptions HeaderJsonOptions = new JsonSerializerOptions(JsonSerialization.JsonSerializerOptions)
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        private readonly RequestDelegate _next;

        public ToastNotificationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task InvokeAsync(HttpContext context)
        {
            // UseNotyf() and UseToastify() both add this middleware; only hook the response once.
            if (!context.Items.ContainsKey(RegisteredKey))
            {
                context.Items[RegisteredKey] = true;
                context.Response.OnStarting(state =>
                {
                    AppendNotificationHeaders((HttpContext)state);
                    return Task.CompletedTask;
                }, context);
            }
            return _next(context);
        }

        internal static void AppendNotificationHeaders(HttpContext httpContext)
        {
            if (!httpContext.Request.IsNotyfAjaxRequest())
            {
                return;
            }

            var tempData = GetTempData(httpContext);
            var changed = false;

            var notyf = httpContext.RequestServices.GetService<INotyfService>();
            if (notyf != null)
            {
                changed |= Deliver(httpContext, tempData, Constants.NotyfResponseHeaderKey, notyf.ReadAllNotifications());
            }

            var toastify = httpContext.RequestServices.GetService<IToastifyService>();
            if (toastify != null)
            {
                changed |= Deliver(httpContext, tempData, Constants.ToastifyResponseHeaderKey, toastify.ReadAllNotifications());
            }

            if (changed)
            {
                tempData?.Save();
            }
        }

        /// <returns>True when TempData was modified.</returns>
        private static bool Deliver<T>(HttpContext httpContext, ITempDataDictionary? tempData, string headerName, IEnumerable<T> raised)
            where T : class
        {
            var pending = raised?.ToList() ?? new List<T>();
            var stored = tempData == null ? null : new TempDataToastNotificationContainer<T>(new TempDataDictionaryService(tempData));
            var changed = false;

            // A redirect hides this response from fetch / XHR (they follow it silently), so keep the toasts
            // in TempData. The follow-up request picks them up below.
            if (IsRedirect(httpContext.Response.StatusCode))
            {
                if (stored == null || pending.Count == 0)
                {
                    return false;
                }
                foreach (var notification in pending)
                {
                    stored.Add(notification);
                }
                return true;
            }

            // Toasts left in TempData by an earlier redirect are shown now.
            if (stored != null && stored.GetAll().Count > 0)
            {
                pending.InsertRange(0, stored.ReadAll());
                changed = true;
            }

            if (pending.Count == 0)
            {
                return changed;
            }

            // Send as many as fit in the header; anything left over waits in TempData for the next page.
            var count = pending.Count;
            string value;
            while (true)
            {
                value = Uri.EscapeDataString(JsonSerializer.Serialize(pending.Take(count).ToList(), HeaderJsonOptions));
                if (value.Length <= MaxHeaderLength || count == 1)
                {
                    break;
                }
                count--;
            }

            if (value.Length <= MaxHeaderLength)
            {
                httpContext.Response.Headers.Append(headerName, value);
                httpContext.Response.Headers.Append(AccessControlExposeHeadersKey, headerName);
            }
            else
            {
                count = 0; // a single toast that is too big for a header
            }

            if (count < pending.Count && stored != null)
            {
                foreach (var notification in pending.Skip(count))
                {
                    stored.Add(notification);
                }
                changed = true;
            }

            return changed;
        }

        private static bool IsRedirect(int statusCode) => statusCode is >= 300 and < 400 and not 304;

        private static ITempDataDictionary? GetTempData(HttpContext httpContext)
        {
            var factory = httpContext.RequestServices.GetService<ITempDataDictionaryFactory>();
            return factory?.GetTempData(httpContext);
        }

        /// <summary>
        /// <see cref="ITempDataService"/> over one TempData dictionary - the middleware already has it,
        /// and IHttpContextAccessor may not be set up in every host.
        /// </summary>
        private sealed class TempDataDictionaryService : ITempDataService
        {
            private readonly ITempDataDictionary _tempData;

            public TempDataDictionaryService(ITempDataDictionary tempData)
            {
                _tempData = tempData;
            }

            public T? Get<T>(string key) where T : class
                => _tempData.TryGetValue(key, out var value) && value is string json ? json.FromJson<T>() : null;

            public T? Peek<T>(string key) where T : class
                => _tempData.Peek(key) is string json ? json.FromJson<T>() : null;

            public void Add(string key, object value) => _tempData[key] = value.ToJson();

            public bool Remove(string key) => _tempData.Remove(key);
        }
    }
}
