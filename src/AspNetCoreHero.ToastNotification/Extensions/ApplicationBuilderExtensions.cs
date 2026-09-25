using AspNetCoreHero.ToastNotification.Middlewares;

// Lives in the Builder namespace so `app.UseNotyf()` works without extra usings.
namespace Microsoft.AspNetCore.Builder
{
    public static class ToastNotificationApplicationBuilderExtensions
    {
        /// <summary>
        /// Shows Notyf notifications raised during AJAX / fetch requests.
        /// </summary>
        public static IApplicationBuilder UseNotyf(this IApplicationBuilder builder)
        {
            System.ArgumentNullException.ThrowIfNull(builder);
            return builder.UseMiddleware<ToastNotificationMiddleware>();
        }

        /// <summary>
        /// Shows Toastify notifications raised during AJAX / fetch requests.
        /// </summary>
        public static IApplicationBuilder UseToastify(this IApplicationBuilder builder)
        {
            System.ArgumentNullException.ThrowIfNull(builder);
            return builder.UseMiddleware<ToastNotificationMiddleware>();
        }
    }
}
