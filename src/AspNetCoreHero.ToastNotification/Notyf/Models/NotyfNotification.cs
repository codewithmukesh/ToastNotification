using AspNetCoreHero.ToastNotification.Abstractions;
using AspNetCoreHero.ToastNotification.Enums;

namespace AspNetCoreHero.ToastNotification.Notyf.Models
{
    public class NotyfNotification : Notification
    {
        public NotyfNotification()
        {
        }

        public NotyfNotification(ToastNotificationType type, string message, int? durationInSeconds) : base(type, message, durationInSeconds)
        {
        }

        public string? Icon { get; set; }
    }
}
