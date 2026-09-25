using AspNetCoreHero.ToastNotification.Abstractions;
using AspNetCoreHero.ToastNotification.Containers;
using AspNetCoreHero.ToastNotification.Enums;
using AspNetCoreHero.ToastNotification.Notyf.Models;
using System.Collections.Generic;

namespace AspNetCoreHero.ToastNotification.Notyf
{
    public class NotyfService : INotyfService
    {
        protected IToastNotificationContainer<NotyfNotification> MessageContainer;

        public NotyfService(IMessageContainerFactory messageContainerFactory)
        {
            MessageContainer = messageContainerFactory.Create<NotyfNotification>();
        }

        public void Success(string message, int? durationInSeconds = null) => Add(ToastNotificationType.Success, message, durationInSeconds);

        public void Error(string message, int? durationInSeconds = null) => Add(ToastNotificationType.Error, message, durationInSeconds);

        public void Information(string message, int? durationInSeconds = null) => Add(ToastNotificationType.Information, message, durationInSeconds);

        public void Warning(string message, int? durationInSeconds = null) => Add(ToastNotificationType.Warning, message, durationInSeconds);

        public void Custom(string message, int? durationInSeconds = null, string? backgroundColor = "black", string? iconClassName = "home", string? className = null)
        {
            MessageContainer.Add(new NotyfNotification(ToastNotificationType.Custom, message ?? string.Empty, durationInSeconds)
            {
                BackgroundColor = backgroundColor,
                Icon = iconClassName,
                ClassName = className
            });
        }

        public IEnumerable<NotyfNotification> GetNotifications() => MessageContainer.GetAll();

        public IEnumerable<NotyfNotification> ReadAllNotifications() => MessageContainer.ReadAll();

        public void RemoveAll() => MessageContainer.RemoveAll();

        private void Add(ToastNotificationType type, string message, int? durationInSeconds)
        {
            MessageContainer.Add(new NotyfNotification(type, message ?? string.Empty, durationInSeconds));
        }
    }
}
