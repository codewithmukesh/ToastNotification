using AspNetCoreHero.ToastNotification.Enums;

namespace AspNetCoreHero.ToastNotification.Abstractions
{
    public class Notification
    {
        /// <summary>
        /// Used by the JSON serializer when notifications are read back from TempData.
        /// </summary>
        public Notification()
        {
        }

        /// <param name="type">The notification type.</param>
        /// <param name="message">The message. Rendered as HTML.</param>
        /// <param name="durationInSeconds">
        /// <c>null</c> uses the globally configured duration. <c>0</c> keeps the toast open until the user dismisses it.
        /// </param>
        public Notification(ToastNotificationType type, string message, int? durationInSeconds)
        {
            Message = message;
            Type = type;
            Duration = durationInSeconds * 1000;
        }

        public string Message { get; set; } = string.Empty;
        public string? BackgroundColor { get; set; }
        public string? ClassName { get; set; }
        public ToastNotificationType Type { get; set; }

        /// <summary>
        /// Duration in milliseconds. <c>null</c> means the global default, <c>0</c> means sticky.
        /// </summary>
        public int? Duration { get; set; }
    }
}
