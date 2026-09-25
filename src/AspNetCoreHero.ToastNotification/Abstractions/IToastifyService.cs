using AspNetCoreHero.ToastNotification.Toastify.Models;
using System.Collections.Generic;

namespace AspNetCoreHero.ToastNotification.Abstractions
{
    /// <summary>
    /// Queues Toastify toasts. They show on the next rendered page (TempData, so redirects are fine)
    /// or, for AJAX / fetch requests, on the response itself.
    /// </summary>
    /// <remarks>
    /// <c>durationInSeconds</c>: <c>null</c> uses the global default, <c>0</c> keeps the toast open until dismissed.
    /// Messages are rendered as HTML - encode user input before passing it in.
    /// </remarks>
    public interface IToastifyService : IToastNotificationService
    {
        void Success(string message, int? durationInSeconds = null);
        void Error(string message, int? durationInSeconds = null);
        void Information(string message, int? durationInSeconds = null);
        void Warning(string message, int? durationInSeconds = null);
        void Custom(string message, int? durationInSeconds = null, string? backgroundColor = "linear-gradient(to right, #00b09b, #96c93d)", string? className = null);
        IEnumerable<ToastifyNotification> GetNotifications();
        IEnumerable<ToastifyNotification> ReadAllNotifications();
        void RemoveAll();
    }
}
