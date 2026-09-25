using AspNetCoreHero.ToastNotification.Abstractions;

namespace AspNetCoreHero.ToastNotification.Containers
{
    public interface IMessageContainerFactory
    {
        IToastNotificationContainer<TMessage> Create<TMessage>() where TMessage : class;
    }
}
