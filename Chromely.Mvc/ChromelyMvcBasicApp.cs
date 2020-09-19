using System;
using Chromely.Core.Host;
using Chromely.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace Chromely.Mvc
{
    /// <summary>
    /// Simplest Chromely app implementation.
    /// Be sure to call base implementations on derived implementations.
    /// </summary>
    public class ChromelyMvcBasicApp : ChromelyMvcApp
    {
        /// <summary>
        /// Configure IoC container contents.
        /// </summary>
        /// <param name="container"></param>
        public override void Configure(IServiceCollection container)
        {
            base.Configure(container);
            container.AddSingleton<IChromelyWindow, ChromelyWindow>();
        }

        /// <summary>
        /// Override to register Chromely events
        /// or use ChromelyEventedApp which already registers some events.
        /// </summary>
        /// <param name="container"></param>
        public override void RegisterEvents(IServiceCollection container)
        {
        }

        /// <summary>
        /// Creates the main window.
        /// </summary>
        /// <returns></returns>
        public override IChromelyWindow CreateWindow()
        {
            var serviceProvider = Container.BuildServiceProvider();
            //Container.AddSingleton<IServiceProvider>(serviceProvider);
            //serviceProvider = Container.BuildServiceProvider();

            return serviceProvider.GetService<IChromelyWindow>();
        }
    }
}