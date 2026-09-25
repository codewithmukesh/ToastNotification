using System.Collections.Generic;
using System.ComponentModel;

namespace AspNetCoreHero.ToastNotification.Notyf.Models
{
    /// <summary>
    /// The options object passed to <c>new Notyf(...)</c> in the browser.
    /// </summary>
    public class NotyfEntity
    {
        public NotyfEntity(int durationInSeconds = 5, NotyfPosition toastPosition = NotyfPosition.BottomRight, bool isDismissible = true)
            : this(new NotyfConfig { DurationInSeconds = durationInSeconds, Position = toastPosition, IsDismissable = isDismissible })
        {
        }

        public NotyfEntity(NotyfConfig? config)
        {
            config ??= new NotyfConfig();
            duration = (config.DurationInSeconds > 0) ? config.DurationInSeconds * 1000 : 5000;
            dismissible = config.IsDismissable;
            ripple = config.HasRippleEffect;
            position = ToPosition(config.Position);
            types = new List<Config>
            {
                ToConfig("success", config.Success),
                ToConfig("error", config.Error),
                ToConfig("warning", config.Warning),
                ToConfig("info", config.Information),
                ToConfig("custom", config.Custom)
            };
        }

        public int duration { get; set; }
        public Position position { get; set; } = new Position { x = "right", y = "bottom" };
        public bool dismissible { get; set; } = true;
        public bool ripple { get; set; } = true;
        public List<Config> types { get; set; } = new List<Config>();

        private static Config ToConfig(string type, ToastTypeStyle? style)
        {
            var config = new Config
            {
                type = type,
                background = style?.BackgroundColor,
                className = string.IsNullOrWhiteSpace(style?.ClassName) ? null : style!.ClassName
            };
            if (!string.IsNullOrWhiteSpace(style?.IconClassName))
            {
                config.icon = new Icon { className = style.IconClassName, tagName = "i" };
            }
            return config;
        }

        private static Position ToPosition(NotyfPosition toastPosition)
        {
            var positionArray = ToDescriptionString(toastPosition).Split('-');
            if (positionArray.Length < 2)
            {
                return new Position { x = "right", y = "bottom" };
            }
            return new Position { x = positionArray[0], y = positionArray[1] };
        }

        private static string ToDescriptionString(NotyfPosition val)
        {
            var field = val.GetType().GetField(val.ToString());
            if (field == null)
            {
                return "right-bottom";
            }
            var attributes = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : "right-bottom";
        }
    }
}
