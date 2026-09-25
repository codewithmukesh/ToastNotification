namespace AspNetCoreHero.ToastNotification.Toastify.Models
{
    public class ToastifyConfig
    {
        /// <summary>
        /// Default duration. <c>0</c> or less falls back to 5 seconds.
        /// </summary>
        public int DurationInSeconds { get; set; }
        public Position Position { get; set; } = Position.Right;
        public Gravity Gravity { get; set; } = Gravity.Bottom;

        /// <summary>
        /// Shows a close button on every toast. Sticky toasts (duration 0) always get one.
        /// </summary>
        public bool IsDismissable { get; set; } = false;

        /// <summary>
        /// Extra CSS class(es) added to every toast.
        /// </summary>
        public string? ClassName { get; set; }

        /// <summary>
        /// Shows notifications raised during AJAX / fetch requests automatically, with or without jQuery.
        /// Requires <c>app.UseToastify()</c>.
        /// </summary>
        public bool AutoHandleAjax { get; set; } = true;

        public ToastTypeStyle Success { get; set; } = new ToastTypeStyle { BackgroundColor = "#388e3c" };
        public ToastTypeStyle Error { get; set; } = new ToastTypeStyle { BackgroundColor = "#d32f2f" };
        public ToastTypeStyle Warning { get; set; } = new ToastTypeStyle { BackgroundColor = "#f57c00" };
        public ToastTypeStyle Information { get; set; } = new ToastTypeStyle { BackgroundColor = "#651fff" };
        public ToastTypeStyle Custom { get; set; } = new ToastTypeStyle { BackgroundColor = "linear-gradient(to right, #00b09b, #96c93d)" };
    }
}
