using AspNetCoreHero.ToastNotification.Abstractions;
using AspNetCoreHero.ToastNotification.Helpers;
using Microsoft.AspNetCore.Http;

namespace AspNetCoreHero.ToastNotification.Containers
{
    internal class MessageContainerFactory : IMessageContainerFactory
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITempDataService _tempDataWrapper;

        public MessageContainerFactory(IHttpContextAccessor httpContextAccessor, ITempDataService tempDataWrapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _tempDataWrapper = tempDataWrapper;
        }

        public IToastNotificationContainer<TMessage> Create<TMessage>()
            where TMessage : class
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null || httpContext.Request.IsNotyfAjaxRequest())
            {
                return new InMemoryNotificationContainer<TMessage>();
            }
            return new TempDataToastNotificationContainer<TMessage>(_tempDataWrapper);
        }
    }
}
