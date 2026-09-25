using System.Text.Json;
using System.Text.Json.Serialization;

namespace AspNetCoreHero.ToastNotification.Helpers
{
    internal static class JsonSerialization
    {
        /// <summary>
        /// camelCase, nulls omitted. Uses the default encoder, which escapes HTML-sensitive
        /// characters (&lt;, &gt;, &amp;, ', ") so the output is safe to embed inside a &lt;script&gt; block.
        /// </summary>
        public static JsonSerializerOptions JsonSerializerOptions { get; } = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public static string ToJson(this object obj)
        {
            return JsonSerializer.Serialize(obj, obj?.GetType() ?? typeof(object), JsonSerializerOptions);
        }

        public static T? FromJson<T>(this string json)
        {
            return JsonSerializer.Deserialize<T>(json, JsonSerializerOptions);
        }
    }
}
