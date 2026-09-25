using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Abstractions;
using AspNetCoreHero.ToastNotification.Containers;
using AspNetCoreHero.ToastNotification.Notyf;
using AspNetCoreHero.ToastNotification.Notyf.Models;
using AspNetCoreHero.ToastNotification.Services;
using AspNetCoreHero.ToastNotification.Toastify;
using AspNetCoreHero.ToastNotification.Toastify.Models;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

// Lives in the DI namespace so `builder.Services.AddNotyf()` works without extra usings.
namespace Microsoft.Extensions.DependencyInjection
{
    public static class ToastNotificationServiceCollectionExtensions
    {
        /// <summary>
        /// Registers <see cref="INotyfService"/>. Add <c>@await Component.InvokeAsync("Notyf")</c> to your layout
        /// and call <c>app.UseNotyf()</c> for AJAX / fetch support.
        /// </summary>
        public static IServiceCollection AddNotyf(this IServiceCollection services, Action<NotyfConfig>? configure = null)
        {
            ArgumentNullException.ThrowIfNull(services);

            var config = new NotyfConfig();
            configure?.Invoke(config);

            services.AddToastNotificationCore();
            services.TryAddScoped<INotyfService, NotyfService>();
            services.Replace(ServiceDescriptor.Singleton(config));
            services.Replace(ServiceDescriptor.Singleton(new NotyfEntity(config)));
            return services;
        }

        /// <summary>
        /// Registers <see cref="IToastifyService"/>. Add <c>@await Component.InvokeAsync("Toastify")</c> to your layout
        /// and call <c>app.UseToastify()</c> for AJAX / fetch support.
        /// </summary>
        public static IServiceCollection AddToastify(this IServiceCollection services, Action<ToastifyConfig>? configure = null)
        {
            ArgumentNullException.ThrowIfNull(services);

            var config = new ToastifyConfig();
            configure?.Invoke(config);

            var entity = new ToastifyEntity(config.DurationInSeconds, config.Gravity, config.Position)
            {
                close = config.IsDismissable,
                className = string.IsNullOrWhiteSpace(config.ClassName) ? null : config.ClassName
            };

            services.AddToastNotificationCore();
            services.TryAddScoped<IToastifyService>(sp => new ToastifyService(sp.GetRequiredService<IMessageContainerFactory>()));
            services.Replace(ServiceDescriptor.Singleton(config));
            services.Replace(ServiceDescriptor.Singleton(entity));
            return services;
        }

        private static void AddToastNotificationCore(this IServiceCollection services)
        {
            // TempData needs a provider; MVC registers one too, TryAdd keeps whichever came first.
            services.TryAddSingleton<ITempDataProvider, CookieTempDataProvider>();
            services.AddHttpContextAccessor();
            services.TryAddSingleton<ITempDataService, TempDataService>();
            services.TryAddSingleton<IMessageContainerFactory, MessageContainerFactory>();
        }
    }
}
