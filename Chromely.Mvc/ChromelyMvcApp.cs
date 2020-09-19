using System;
using System.Collections.Generic;
using System.IO;
using Chromely.Core;
using Chromely.Core.Configuration;
using Chromely.Core.Defaults;
using Chromely.Core.Host;
using Chromely.Core.Infrastructure;
using Chromely.Core.Logging;
using Chromely.Core.Network;
using Chromely.Native;
using Microsoft.Extensions.DependencyInjection;

namespace Chromely.Mvc
{
    public abstract class ChromelyMvcApp
    {
        protected IServiceCollection _container;

        public virtual IServiceCollection Container
        {
            get
            {
                EnsureContainerValid(_container);
                return _container;
            }
        }

        public virtual IChromelyConfiguration Configuration
        {
            get
            {
                EnsureContainerValid(Container);
                var config = Container.BuildServiceProvider().GetService<IChromelyConfiguration>();
                return config;
            }
        }

        public virtual IChromelyWindow Window
        {
            get
            {
                EnsureContainerValid(Container);
                var window = Container.BuildServiceProvider().GetService<IChromelyWindow>();
                return window;
            }
        }

        public virtual void Initialize(IServiceCollection container, IChromelyAppSettings appSettings, IChromelyConfiguration config, IChromelyLogger chromelyLogger)
        {
            EnsureExpectedWorkingDirectory();

            #region Container

            _container = container;
            if (_container == null)
            {
                _container = new ServiceCollection();
            }

            #endregion

            #region Configuration 

            if (config == null)
            {
                var configurator = new ConfigurationHandler();
                config = configurator.Parse<DefaultConfiguration>();
            }

            if (config == null)
            {
                config = DefaultConfiguration.CreateForRuntimePlatform();
            }

            InitConfiguration(config);
            config.Platform = ChromelyRuntime.Platform;

            #endregion

            #region Application/User Settings

            if (appSettings == null)
            {
                appSettings = new DefaultAppSettings(config.AppName);
            }

            var currentAppSettings = new CurrentAppSettings();
            currentAppSettings.Properties = appSettings;
            ChromelyAppUser.App = currentAppSettings;
            ChromelyAppUser.App.Properties.Read(config);

            #endregion

            #region Logger

            if (chromelyLogger == null)
            {
                chromelyLogger = new SimpleLogger();
            }

            var defaultLogger = new DefaultLogger();
            defaultLogger.Log = chromelyLogger;
            Logger.Instance = defaultLogger;

            #endregion

            // Register all primary objects
            _container.AddSingleton<IChromelyContainer>(new ChromelyServiceCollectionContainer(_container));
            _container.AddSingleton(_container);
            _container.AddSingleton(appSettings);
            _container.AddSingleton(config);
            _container.AddSingleton(chromelyLogger);
            _container.AddSingleton(NativeHostFactory.GetNativeHost(config));
            _container.AddChromelyMvcWithDefaultRoutes();
        }

        public virtual void Configure(IServiceCollection container)
        {
            EnsureContainerValid(container);

            container.AddSingleton<IChromelyRequestTaskRunner, DefaultMvcRequestTaskRunner>();
            container.AddSingleton<IChromelyCommandTaskRunner, DefaultCommandTaskRunner>();
        }

        public abstract void RegisterEvents(IServiceCollection container);

        public abstract IChromelyWindow CreateWindow();

        protected void InitConfiguration(IChromelyConfiguration config)
        {
            if (config == null)
            {
                throw new Exception("Configuration cannot be null.");
            }

            if (config.UrlSchemes == null) config.UrlSchemes = new List<UrlScheme>();
            if (config.ControllerAssemblies == null) config.ControllerAssemblies = new List<ControllerAssemblyInfo>();
            if (config.CommandLineArgs == null) config.CommandLineArgs = new Dictionary<string, string>();
            if (config.CommandLineOptions == null) config.CommandLineOptions = new List<string>();
            if (config.CustomSettings == null) config.CustomSettings = new Dictionary<string, string>();
            //if (config.WindowOptions == null) config.WindowOptions = new Configuration.WindowOptions();
        }

        protected void EnsureContainerValid(IServiceCollection container)
        {
            if (container == null)
            {
                throw new Exception("Container cannot be null. Initialize method must be called first.");
            }
        }

        /// <summary>
        /// Using local resource handling requires files to be relative to the 
        /// Expected working directory
        /// For example, if the app is launched via the taskbar the working directory gets changed to
        /// C:\Windows\system32
        /// This needs to be changed to the right one.
        /// </summary>
        protected static void EnsureExpectedWorkingDirectory()
        {
            try
            {
                var appDirectory = AppDomain.CurrentDomain.BaseDirectory;
                Directory.SetCurrentDirectory(appDirectory);
            }
            catch (Exception exception)
            {
                Logger.Instance.Log.Error(exception);
            }
        }
    }
}