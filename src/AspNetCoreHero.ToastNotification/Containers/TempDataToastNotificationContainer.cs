using AspNetCoreHero.ToastNotification.Abstractions;
using System.Collections.Generic;

namespace AspNetCoreHero.ToastNotification.Containers
{
    /// <summary>
    /// Stores notifications in TempData so they survive redirects. Used for regular (non-AJAX) requests.
    /// </summary>
    public class TempDataToastNotificationContainer<TMessage> : IToastNotificationContainer<TMessage> where TMessage : class
    {
        // One key per notification type so Notyf and Toastify never read each other's messages.
        internal static readonly string Key = "AspNetCoreHero.ToastNotification." + typeof(TMessage).Name;
        private readonly ITempDataService _tempDataWrapper;

        public TempDataToastNotificationContainer(ITempDataService tempDataWrapper)
        {
            _tempDataWrapper = tempDataWrapper;
        }

        public void Add(TMessage message)
        {
            var messages = _tempDataWrapper.Get<List<TMessage>>(Key) ?? new List<TMessage>();
            messages.Add(message);
            _tempDataWrapper.Add(Key, messages);
        }

        public void RemoveAll()
        {
            _tempDataWrapper.Remove(Key);
        }

        public IList<TMessage> GetAll()
        {
            return _tempDataWrapper.Peek<List<TMessage>>(Key) ?? new List<TMessage>();
        }

        public IList<TMessage> ReadAll()
        {
            var messages = GetAll();
            RemoveAll();
            return messages;
        }
    }
}
