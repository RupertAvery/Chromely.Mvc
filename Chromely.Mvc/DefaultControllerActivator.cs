using System;
using Microsoft.Extensions.DependencyInjection;

namespace Chromely.Mvc
{
    public class DefaultControllerActivator : IControllerActivator
    {
        private readonly IServiceCollection _serviceCollection;
        private IServiceProvider _serviceProvider;

        private IServiceProvider ServiceProvider
        {
            get
            {
                if (_serviceProvider == null)
                {
                    _serviceProvider = _serviceCollection.BuildServiceProvider();
                }

                return _serviceProvider;
            }
        }

        public DefaultControllerActivator(IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
        }

        public object Create(ActionContext actionContext)
        {
            return ServiceProvider.GetService(actionContext.ControllerType);
        }

        public object Release(ActionContext actionContext, object controller)
        {
            return null;
        }
    }
}