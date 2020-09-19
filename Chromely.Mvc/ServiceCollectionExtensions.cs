using Chromely.Core;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Microsoft.Extensions.Logging;

//using Microsoft.Extensions.Logging;
//using Chromely.Core.Configuration;

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
                .AddSingleton<IActionBuilder, DefaultActionBuilder>()
                .AddSingleton<IModelBinder, DefaultModelBinder>()
                .AddSingleton<IRouteResolver, DefaultRouteResolver>()
                .AddSingleton<IControllerActivator, DefaultControllerActivator>()
                .AddLogging(builder => { builder.AddDebug(); })
                ;


            routeAction(routeCollection);

            return services;
        }

        /// <summary>
        /// Adds the default types needed for Chomely.Mvc using default routing
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddChromelyMvcWithDefaultRoutes(this IServiceCollection services)
        {
            return services.AddChromelyMvc((routes) =>
            {
                routes.MapRoute("default", "/{controller}/{action}");
            });
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
        /// Scans the calling assembly for controllers and registers the matching types to the <see cref="IControllerFactory"/>
        /// and the service collection as transient services.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddControllers(this IServiceCollection services)
        {
            var assembly = Assembly.GetCallingAssembly();
            return AddControllers(services, assembly);
        }

        /// <summary>
        /// Scans the specififed assembly for controllers and registers the matching types to the <see cref="IControllerFactory"/>
        /// and the service collection as transient services.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static IServiceCollection AddControllers(this IServiceCollection services, Assembly assembly)
        {
            var controllerFactory = services.BuildServiceProvider().GetService<IControllerFactory>();
            foreach (var type in assembly.GetTypes<Controller>())
            {
                controllerFactory.Add(type);
                services.AddTransient(type);
            }
            return services;
        }
    }

}
