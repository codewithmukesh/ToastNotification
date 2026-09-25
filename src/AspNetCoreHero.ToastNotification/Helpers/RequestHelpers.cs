using Microsoft.AspNetCore.Http;
using System;

namespace AspNetCoreHero.ToastNotification.Helpers
{
    public static class RequestHelpers
    {
        /// <summary>
        /// True for AJAX-style requests: jQuery / XHR / fetch (<c>X-Requested-With</c>) and htmx (<c>HX-Request</c>).
        /// Notifications for these requests are returned in a response header instead of TempData.
        /// </summary>
        public static bool IsNotyfAjaxRequest(this HttpRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);
            return !string.IsNullOrWhiteSpace(request.Headers[Constants.RequestHeaderKey])
                || !string.IsNullOrWhiteSpace(request.Headers[Constants.HtmxRequestHeaderKey]);
        }
    }
}
