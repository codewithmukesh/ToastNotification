using System;
using System.ComponentModel;
using System.Reflection;

namespace AspNetCoreHero.ToastNotification.Extensions
{
    public static class EnumExtensions
    {
        public static string ToDescriptionString<T>(this T source) where T : Enum
        {
            var name = source.ToString();
            var attribute = source.GetType().GetField(name)?.GetCustomAttribute<DescriptionAttribute>(false);
            return attribute?.Description ?? name;
        }
    }
}
