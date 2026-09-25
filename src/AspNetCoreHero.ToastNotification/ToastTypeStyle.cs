namespace AspNetCoreHero.ToastNotification
{
    /// <summary>
    /// Default look of one notification type (Success, Error, Warning, Information or Custom).
    /// </summary>
    public class ToastTypeStyle
    {
        /// <summary>
        /// Any valid CSS background, e.g. <c>#28a745</c>, <c>orange</c> or a gradient.
        /// </summary>
        public string? BackgroundColor { get; set; }

        /// <summary>
        /// Extra CSS class(es) added to every toast of this type. Separate multiple classes with spaces.
        /// </summary>
        public string? ClassName { get; set; }

        /// <summary>
        /// Icon CSS class(es), e.g. <c>fa fa-warning</c>. Notyf only; ignored by Toastify.
        /// </summary>
        public string? IconClassName { get; set; }
    }
}
