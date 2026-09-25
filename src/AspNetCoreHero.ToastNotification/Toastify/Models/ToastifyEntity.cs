using AspNetCoreHero.ToastNotification.Extensions;
using System;

namespace AspNetCoreHero.ToastNotification.Toastify.Models
{
    /// <summary>
    /// The options object passed to <c>Toastify(...)</c> in the browser.
    /// </summary>
    public class ToastifyEntity
    {
        public ToastifyEntity(int durationInSeconds, Gravity gravity = Gravity.Bottom, Position position = Position.Right)
        {
            this.duration = durationInSeconds > 0 ? durationInSeconds * 1000 : 5000;
            this.gravity = gravity.ToDescriptionString();
            this.position = position.ToDescriptionString();
        }

        public string? text { get; set; }
        public int? duration { get; set; }
        public Uri? destination { get; set; }
        public bool newWindow { get; set; } = true;
        public bool close { get; set; } = false;
        public string gravity { get; set; } = "bottom";
        public string? backgroundColor { get; set; }
        public bool stopOnFocus { get; set; } = true;
        public string position { get; set; } = "right";
        public Uri? avatar { get; set; }
        public string? className { get; set; }
    }
}
