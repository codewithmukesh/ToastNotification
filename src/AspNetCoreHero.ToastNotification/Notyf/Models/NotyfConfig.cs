namespace AspNetCoreHero.ToastNotification
{
    public class NotyfConfig
    {
        public const string DefaultFontAwesomeUrl = "https://stackpath.bootstrapcdn.com/font-awesome/4.7.0/css/font-awesome.min.css";

        public int DurationInSeconds { get; set; }
        public NotyfPosition Position { get; set; } = NotyfPosition.BottomRight;
        public bool IsDismissable { get; set; } = false;
        public bool HasRippleEffect { get; set; } = true;

        /// <summary>
        /// Adds the Font Awesome 4.7 stylesheet used by the Warning/Information icons.
        /// Set to <c>false</c> if your layout already loads Font Awesome (or you use other icons).
        /// </summary>
        public bool IncludeFontAwesome { get; set; } = true;

        /// <summary>
        /// Stylesheet loaded when <see cref="IncludeFontAwesome"/> is <c>true</c>. Point it at a local copy to avoid the CDN.
        /// </summary>
        public string FontAwesomeUrl { get; set; } = DefaultFontAwesomeUrl;

        /// <summary>
        /// Renders toasts right-to-left (icon and close button are mirrored).
        /// </summary>
        public bool IsRtl { get; set; } = false;

        /// <summary>
        /// Extra CSS class(es) added to every toast.
        /// </summary>
        public string? ClassName { get; set; }

        /// <summary>
        /// Shows notifications raised during AJAX / fetch requests automatically, with or without jQuery.
        /// Requires <c>app.UseNotyf()</c>.
        /// </summary>
        public bool AutoHandleAjax { get; set; } = true;

        public ToastTypeStyle Success { get; set; } = new ToastTypeStyle { BackgroundColor = "#28a745" };
        public ToastTypeStyle Error { get; set; } = new ToastTypeStyle { BackgroundColor = "#dc3545" };
        public ToastTypeStyle Warning { get; set; } = new ToastTypeStyle { BackgroundColor = "orange", ClassName = "text-dark", IconClassName = "fa fa-warning text-dark" };
        public ToastTypeStyle Information { get; set; } = new ToastTypeStyle { BackgroundColor = "#17a2b8", IconClassName = "fa fa-info text-white" };
        public ToastTypeStyle Custom { get; set; } = new ToastTypeStyle { BackgroundColor = "black" };
    }
}
