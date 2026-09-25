using AspNetCoreHero.ToastNotification.Abstractions;
using AspNetCoreHero.ToastNotification.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace AspNetCoreHero.ToastNotification.Services
{
    public class TempDataService : ITempDataService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITempDataDictionaryFactory _tempDataDictionaryFactory;

        public TempDataService(ITempDataDictionaryFactory tempDataDictionaryFactory, IHttpContextAccessor httpContextAccessor)
        {
            _tempDataDictionaryFactory = tempDataDictionaryFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Gets or sets <see cref="ITempDataDictionary"/>/>.
        /// </summary>
        private ITempDataDictionary TempData => _tempDataDictionaryFactory.GetTempData(_httpContextAccessor.HttpContext
            ?? throw new System.InvalidOperationException("TempData is only available during an HTTP request."));

        public T? Get<T>(string key) where T : class
        {
            if (TempData.ContainsKey(key) && TempData[key] is string json)
            {
                return json.FromJson<T>();
            }
            return null;
        }

        public T? Peek<T>(string key) where T : class
        {
            if (TempData.ContainsKey(key) && TempData.Peek(key) is string json)
            {
                return json.FromJson<T>();
            }
            return null;
        }

        public void Add(string key, object value)
        {
            TempData[key] = value.ToJson();
        }

        public bool Remove(string key)
        {
            return TempData.ContainsKey(key) && TempData.Remove(key);
        }
    }
}
