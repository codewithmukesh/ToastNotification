using AspNetCoreHero.ToastNotification.Toastify.Models;
using System.Collections.Generic;

namespace AspNetCoreHero.ToastNotification.Toastify
{
    public class ToastifyViewModel
    {
        public ToastifyEntity Configuration { get; set; } = new ToastifyEntity(0);

        public IEnumerable<ToastifyNotification> Notifications { get; set; } = new List<ToastifyNotification>();

        /// <summary>
        /// Everything the client script needs (Toastify options, per-type styles, notifications) as script-safe JSON.
        /// Rendered in a <c>&lt;script type="application/json"&gt;</c> block, so no inline JavaScript is needed.
        /// </summary>
        public string ClientData { get; set; } = "{}";

        /// <summary>
        /// Optional Content-Security-Policy nonce added to the rendered &lt;script&gt; tags.
        /// </summary>
        public string? Nonce { get; set; }
    }
}
