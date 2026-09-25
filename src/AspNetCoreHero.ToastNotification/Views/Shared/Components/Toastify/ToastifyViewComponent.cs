using AspNetCoreHero.ToastNotification.Abstractions;
using AspNetCoreHero.ToastNotification.Helpers;
using AspNetCoreHero.ToastNotification.Toastify;
using AspNetCoreHero.ToastNotification.Toastify.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace AspNetCoreHero.ToastNotification.Views.Shared.Components.Toastify
{
    /// <summary>
    /// Renders queued Toastify toasts. Add <c>@await Component.InvokeAsync("Toastify")</c> to your layout, just before <c>&lt;/body&gt;</c>.
    /// </summary>
    [ViewComponent(Name = "Toastify")]
    public class ToastifyViewComponent : ViewComponent
    {
        private readonly IToastifyService _service;
        private readonly ToastifyConfig _config;

        public ToastifyViewComponent(IToastifyService service, ToastifyEntity options, ToastifyConfig? config = null)
        {
            _service = service;
            _options = options;
            _config = config ?? new ToastifyConfig();
        }

        public ToastifyEntity _options { get; }

        /// <param name="nonce">Optional CSP nonce: <c>@await Component.InvokeAsync("Toastify", new { nonce = "..." })</c></param>
        public IViewComponentResult Invoke(string? nonce = null)
        {
            var notifications = _service.ReadAllNotifications()?.ToList() ?? new List<ToastifyNotification>();
            var model = new ToastifyViewModel
            {
                Configuration = _options,
                Notifications = notifications,
                ClientData = new
                {
                    config = _options,
                    options = BuildClientOptions(_config),
                    notifications
                }.ToJson(),
                Nonce = nonce
            };
            return View("Default", model);
        }

        internal static object BuildClientOptions(ToastifyConfig config)
        {
            return new
            {
                autoHandleAjax = config.AutoHandleAjax,
                types = new
                {
                    success = ToStyle(config.Success),
                    error = ToStyle(config.Error),
                    warning = ToStyle(config.Warning),
                    info = ToStyle(config.Information),
                    custom = ToStyle(config.Custom)
                }
            };
        }

        private static object ToStyle(ToastTypeStyle? style)
        {
            return new { backgroundColor = style?.BackgroundColor, className = style?.ClassName };
        }
    }
}
