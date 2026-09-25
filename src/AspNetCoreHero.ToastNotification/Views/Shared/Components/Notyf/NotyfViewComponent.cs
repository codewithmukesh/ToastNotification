using AspNetCoreHero.ToastNotification.Abstractions;
using AspNetCoreHero.ToastNotification.Helpers;
using AspNetCoreHero.ToastNotification.Notyf;
using AspNetCoreHero.ToastNotification.Notyf.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace AspNetCoreHero.ToastNotification.Views.Shared.Components.Notyf
{
    /// <summary>
    /// Renders queued Notyf toasts. Add <c>@await Component.InvokeAsync("Notyf")</c> to your layout, just before <c>&lt;/body&gt;</c>.
    /// </summary>
    [ViewComponent(Name = "Notyf")]
    public class NotyfViewComponent : ViewComponent
    {
        private readonly INotyfService _service;
        private readonly NotyfConfig _config;

        public NotyfViewComponent(INotyfService service, NotyfEntity options, NotyfConfig? config = null)
        {
            _service = service;
            _options = options;
            _config = config ?? new NotyfConfig();
        }

        public NotyfEntity _options { get; }

        /// <param name="nonce">Optional CSP nonce: <c>@await Component.InvokeAsync("Notyf", new { nonce = "..." })</c></param>
        public IViewComponentResult Invoke(string? nonce = null)
        {
            var notifications = _service.ReadAllNotifications()?.ToList() ?? new List<NotyfNotification>();
            var model = new NotyfViewModel
            {
                Configuration = _options.ToJson(),
                Notifications = notifications,
                ClientData = new
                {
                    config = _options,
                    options = BuildClientOptions(_config),
                    notifications
                }.ToJson(),
                IncludeFontAwesome = _config.IncludeFontAwesome && !string.IsNullOrWhiteSpace(_config.FontAwesomeUrl),
                FontAwesomeUrl = _config.FontAwesomeUrl,
                Nonce = nonce
            };
            return View("Default", model);
        }

        internal static object BuildClientOptions(NotyfConfig config)
        {
            var classNames = new List<string>();
            if (!string.IsNullOrWhiteSpace(config.ClassName)) classNames.Add(config.ClassName.Trim());
            if (config.IsRtl) classNames.Add("notyf__toast--rtl");
            if (config.Position is NotyfPosition.TopFullWidth or NotyfPosition.BottomFullWidth) classNames.Add("notyf__toast--full-width");
            return new
            {
                className = string.Join(" ", classNames),
                autoHandleAjax = config.AutoHandleAjax
            };
        }
    }
}
