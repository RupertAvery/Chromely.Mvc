using Chromely.Core;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Chromely.Mvc
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the default types needed for Chromely.Mvc
        /// </summary>
        /// <param name="services"></param>
        /// <param name="routeAction"></param>
        /// <returns></returns>
        public static IServiceCollection AddChromelyMvc(this IServiceCollection services, Action<RouteCollection> routeAction)
        {
            var routeCollection = new RouteCollection();
            var controllerFactory = new DefaultControllerFactory();

            services
                .AddSingleton<IControllerFactory>(provider => controllerFactory)
                .AddSingleton<RouteCollection>(provider => routeCollection)
                .AddSingleton<MvcCefSharpBoundObject>()
                .AddSingleton<IActionBuilder, DefaultActionBuilder>()
                .AddSingleton<IModelBinder, DefaultModelBinder>()
                .AddSingleton<IControllerActivator, DefaultControllerActivator>()
                .AddSingleton<IRouteResolver, DefaultRouteResolver>()
                .AddSingleton<IRequestHandler, DefaultRequestHandler>();

            routeAction(routeCollection);

            return services;
        }

        /// <summary>
        /// Adds the default types needed for Chomely.Mvc using default routing
        /// </summary>
        /// <param name="services"></param>
        /// <param name="routeAction"></param>
        /// <returns></returns>
        public static IServiceCollection AddChromelyMvcWithDefaultRoutes(this IServiceCollection services)
        {
            return services.AddChromelyMvc((routes) =>
            {
                routes.MapRoute("default", "/{controller}/{action}");
            });
        }

        /// <summary>
        /// Creates and registers an instance of the <see cref="MvcCefSharpBoundObject"></see> as the bound object of a <see cref="ChromelyJshander"></see>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="objectNameToBind"></param>
        /// <param name="registerAsync"></param>
        /// <returns></returns>
        public static IServiceCollection UseDefaultMvcBoundObject(this IServiceCollection services, ChromelyConfiguration configuration, string objectNameToBind = "boundControllerAsync", bool registerAsync = true)
        {
            var boundObject = services.BuildServiceProvider().GetService<MvcCefSharpBoundObject>();
            configuration.RegisterJsHandler(new ChromelyJsHandler(objectNameToBind, boundObject, null, registerAsync));
            return services;
        }

        /// <summary>
        /// Helper method to register multiple services in a fluent manner
        /// </summary>
        /// <param name="services"></param>
        /// <param name="registerAction"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterServices(this IServiceCollection services, Action<IServiceCollection> registerAction)
        {
            registerAction(services);
            return services;
        }

        /// <summary>
        /// Scans the specififed aassembly using the registered <see cref="IControllerFactory"></see> and adds resulting types
        /// to the service collection as a transient service
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static IServiceCollection UseControllersFromAssembly(this IServiceCollection services, Assembly assembly)
        {
            var controllerFactory = services.BuildServiceProvider().GetService<IControllerFactory>();
            foreach (var type in controllerFactory.ScanAssembly(assembly))
            {
                services.AddTransient(type);
            }
            return services;
        }

    }

}
