using System;
using System.Collections.Generic;
using System.Linq;
using Chromely.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Chromely.Mvc
{
    public class ChromelyServiceCollectionContainer : IChromelyContainer
    {
        private readonly IServiceCollection _serviceCollection;
        private readonly Dictionary<string, Type> _keys;

        public ChromelyServiceCollectionContainer(IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
            _keys = _serviceCollection.ToDictionary(descriptor => descriptor.ServiceType.Name, descriptor => descriptor.ServiceType);
        }

        public bool IsRegistered(Type service, string key)
        {
            return (_serviceCollection.Any(x => x.ServiceType == service));
        }

        public bool IsRegistered<TService>(string key)
        {
            return (_serviceCollection.Any(x => x.ServiceType == typeof(TService)));
        }

        public string[] GetKeys(Type service)
        {
            throw new NotImplementedException();
        }

        public void RegisterSingleton(Type service, string key, Type implementation)
        {
            _serviceCollection.AddSingleton(service, implementation);
            _keys.Add(key, service);
        }

        public void RegisterByTypeSingleton(Type service, Type implementation)
        {
            _serviceCollection.AddSingleton(service, implementation);
            _keys.Add(service.Name, service);
        }

        public void RegisterSingleton<TImplementation>(string key)
        {
            throw new NotImplementedException();
        }

        public void RegisterSingleton<TService, TImplementation>(string key) where TImplementation : TService
        {
            throw new NotImplementedException();
        }

        public void RegisterByTypeSingleton<TService, TImplementation>() where TImplementation : TService
        {
            throw new NotImplementedException();
        }

        public void RegisterInstance(Type service, string key, object instance)
        {
            throw new NotImplementedException();
        }

        public void RegisterInstance<TService>(string key, TService instance)
        {
            throw new NotImplementedException();
        }

        public void RegisterPerRequest(Type service, string key, Type implementation)
        {
            throw new NotImplementedException();
        }

        public void RegisterPerRequest<TService>(string key)
        {
            throw new NotImplementedException();
        }

        public void RegisterPerRequest<TService, TImplementation>(string key) where TImplementation : TService
        {
            throw new NotImplementedException();
        }

        public bool UnregisterHandler(Type service, string key)
        {
            throw new NotImplementedException();
        }

        public bool UnregisterHandler<TService>(string key)
        {
            throw new NotImplementedException();
        }

        public object GetInstance(Type service, string key)
        {
            return _serviceCollection.BuildServiceProvider().GetService(service);
        }

        public TService GetInstance<TService>(string key)
        {
            return _serviceCollection.BuildServiceProvider().GetService<TService>();
        }

        public object[] GetAllInstances(Type service)
        {
            return _serviceCollection.BuildServiceProvider().GetServices(service).ToArray();
        }

        public TService[] GetAllInstances<TService>()
        {
            return _serviceCollection.BuildServiceProvider().GetServices<TService>().ToArray();
        }
    }
}