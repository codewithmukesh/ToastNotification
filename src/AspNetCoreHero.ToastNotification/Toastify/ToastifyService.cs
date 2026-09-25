using AspNetCoreHero.ToastNotification.Abstractions;
using AspNetCoreHero.ToastNotification.Containers;
using AspNetCoreHero.ToastNotification.Enums;
using AspNetCoreHero.ToastNotification.Toastify.Models;
using System.Collections.Generic;

namespace AspNetCoreHero.ToastNotification.Toastify
{
    public class ToastifyService : IToastifyService
    {
        private readonly IToastNotificationContainer<ToastifyNotification> _container;

        public ToastifyService(IMessageContainerFactory messageContainerFactory)
            : this(messageContainerFactory.Create<ToastifyNotification>())
        {
        }

        public ToastifyService(IToastNotificationContainer<ToastifyNotification> container)
        {
            _container = container;
        }

        public void Success(string message, int? durationInSeconds = null) => Add(ToastNotificationType.Success, message, durationInSeconds);

        public void Error(string message, int? durationInSeconds = null) => Add(ToastNotificationType.Error, message, durationInSeconds);

        public void Information(string message, int? durationInSeconds = null) => Add(ToastNotificationType.Information, message, durationInSeconds);

        public void Warning(string message, int? durationInSeconds = null) => Add(ToastNotificationType.Warning, message, durationInSeconds);

        public void Custom(string message, int? durationInSeconds = null, string? backgroundColor = "linear-gradient(to right, #00b09b, #96c93d)", string? className = null)
        {
            _container.Add(new ToastifyNotification(ToastNotificationType.Custom, message ?? string.Empty, durationInSeconds)
            {
                BackgroundColor = backgroundColor,
                ClassName = className
            });
        }

        public IEnumerable<ToastifyNotification> GetNotifications() => _container.GetAll();

        public IEnumerable<ToastifyNotification> ReadAllNotifications() => _container.ReadAll();

        public void RemoveAll() => _container.RemoveAll();

        private void Add(ToastNotificationType type, string message, int? durationInSeconds)
        {
            _container.Add(new ToastifyNotification(type, message ?? string.Empty, durationInSeconds));
        }
    }
}
