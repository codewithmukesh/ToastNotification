namespace AspNetCoreHero.ToastNotification.Abstractions
{
    public interface ITempDataService
    {
        T? Get<T>(string key) where T : class;
        T? Peek<T>(string key) where T : class;
        void Add(string key, object value);
        bool Remove(string key);
    }
}
