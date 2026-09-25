using AspNetCoreHero.ToastNotification.Abstractions;
using AspNetCoreHero.ToastNotification.Enums;

namespace AspNetCoreHero.ToastNotification.Toastify.Models
{
    public class ToastifyNotification : Notification
    {
        public ToastifyNotification()
        {
        }

        public ToastifyNotification(ToastNotificationType type, string message, int? durationInSeconds) : base(type, message, durationInSeconds)
        {
        }
    }
}
