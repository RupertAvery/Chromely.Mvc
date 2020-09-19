using System;
using System.Text;
using Caliburn.Light;
using Chromely.Core;
using Chromely.Core.Configuration;
using Chromely.Core.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Chromely.Mvc
{
    public sealed class MvcAppBuilder
    {
        private IServiceCollection _container;
        private IChromelyConfiguration _config;
        private IChromelyAppSettings _appSettings;
        private IChromelyLogger _logger;
        private ChromelyMvcApp _chromelyApp;
        private int _stepCompleted;

        private MvcAppBuilder()
        {
            _stepCompleted = 1;
        }

        public static MvcAppBuilder Create()
        {
            var appBuilder = new MvcAppBuilder();
            return appBuilder;
        }

        public MvcAppBuilder UseContainer<T>(IServiceCollection container = null) where T : IServiceCollection
        {
            _container = container;
            if (_container == null)
            {
                EnsureIsDerivedType(typeof(IServiceCollection), typeof(T));
                _container = (T)Activator.CreateInstance(typeof(T));
            }

            return this;
        }

        public MvcAppBuilder UseAppSettings<T>(IChromelyAppSettings appSettings = null) where T : IChromelyAppSettings
        {
            _appSettings = appSettings;
            if (_appSettings == null)
            {
                EnsureIsDerivedType(typeof(IChromelyAppSettings), typeof(T));
                _appSettings = (T)Activator.CreateInstance(typeof(T));
            }

            return this;
        }

        public MvcAppBuilder UseLogger<T>(IChromelyLogger logger = null) where T : IChromelyLogger
        {
            _logger = logger;
            if (_logger == null)
            {
                EnsureIsDerivedType(typeof(IChromelyLogger), typeof(T));
                _logger = (T)Activator.CreateInstance(typeof(T));
            }

            return this;
        }

        public MvcAppBuilder UseConfiguration<T>(IChromelyConfiguration config = null) where T : IChromelyConfiguration
        {
            _config = config;
            if (_config == null)
            {
                EnsureIsDerivedType(typeof(IChromelyConfiguration), typeof(T));
                _config = (T)Activator.CreateInstance(typeof(T));
            }

            return this;
        }

        public MvcAppBuilder UseApp<T>(ChromelyMvcApp chromelyApp = null) where T : ChromelyMvcApp
        {
            if (_stepCompleted != 1)
            {
                throw new Exception("Invalid order: step 1: Contructor must be completed before step 2.");
            }

            _chromelyApp = chromelyApp;
            if (_chromelyApp == null)
            {
                EnsureIsDerivedType(typeof(ChromelyMvcApp), typeof(T));
                _chromelyApp = (T)Activator.CreateInstance(typeof(T));
            }

            _stepCompleted = 2;
            return this;
        }

        public MvcAppBuilder Build()
        {
            if (_stepCompleted != 2)
            {
                throw new Exception("Invalid order: step 2: UseApp must be completed before step 3.");
            }

            if (_chromelyApp == null)
            {
                throw new Exception($"ChromelyApp {nameof(_chromelyApp)} cannot be null.");
            }

            _chromelyApp.Initialize(_container, _appSettings, _config, _logger);
            _container = _chromelyApp.Container;
            _config = _chromelyApp.Configuration;
            _chromelyApp.Configure(_container);
            _chromelyApp.RegisterEvents(_container);

            _stepCompleted = 3;
            return this;
        }

        public void Run(string[] args)
        {
            if (_stepCompleted != 3)
            {
                throw new Exception("Invalid order: step 3: Build must be completed before step 4.");
            }

            try
            {
                using (var window = _chromelyApp.CreateWindow())
                {
                    try
                    {
                        window.Run(args);
                    }
                    catch (Exception exception)
                    {
                        Logger.Instance.Log.Error(exception);
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Instance.Log.Error(exception);
            }
        }

        private void EnsureIsDerivedType(Type baseType, Type derivedType)
        {
            if (baseType == derivedType)
            {
                throw new Exception($"Cannot specify the base type {baseType.Name} itself as generic type parameter.");
            }

            if (!baseType.IsAssignableFrom(derivedType))
            {
                throw new Exception($"Type {derivedType.Name} must implement {baseType.Name}.");
            }

            if (derivedType.IsAbstract || derivedType.IsInterface)
            {
                throw new Exception($"Type {derivedType.Name} cannot be an interface or abstract class.");
            }
        }
    }
}
