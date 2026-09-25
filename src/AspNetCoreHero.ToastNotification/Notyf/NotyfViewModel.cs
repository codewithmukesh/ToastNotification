using AspNetCoreHero.ToastNotification.Notyf.Models;
using System.Collections.Generic;

namespace AspNetCoreHero.ToastNotification.Notyf
{
    public class NotyfViewModel
    {
        /// <summary>
        /// JSON options passed to <c>new Notyf(...)</c>.
        /// </summary>
        public string Configuration { get; set; } = "{}";

        public IEnumerable<NotyfNotification> Notifications { get; set; } = new List<NotyfNotification>();

        /// <summary>
        /// Everything the client script needs (Notyf options, glue options, notifications) as script-safe JSON.
        /// Rendered in a <c>&lt;script type="application/json"&gt;</c> block, so no inline JavaScript is needed.
        /// </summary>
        public string ClientData { get; set; } = "{}";

        public bool IncludeFontAwesome { get; set; }
        public string? FontAwesomeUrl { get; set; }

        /// <summary>
        /// Optional Content-Security-Policy nonce added to the rendered &lt;script&gt; tags.
        /// </summary>
        public string? Nonce { get; set; }
    }
}
