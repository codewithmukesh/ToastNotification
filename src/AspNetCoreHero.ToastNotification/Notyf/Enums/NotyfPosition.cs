using System.ComponentModel;

namespace AspNetCoreHero.ToastNotification
{
    public enum NotyfPosition
    {
        [Description("right-top")]
        TopRight,
        [Description("right-bottom")]
        BottomRight,
        [Description("left-bottom")]
        BottomLeft,
        [Description("left-top")]
        TopLeft,
        [Description("center-top")]
        TopCenter,
        [Description("center-bottom")]
        BottomCenter,
        [Description("center-top")]
        TopFullWidth,
        [Description("center-bottom")]
        BottomFullWidth
    }
}
