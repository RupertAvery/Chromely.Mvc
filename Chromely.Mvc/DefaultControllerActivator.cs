using System;

namespace Chromely.Mvc
{
    public class DefaultControllerActivator : IControllerActivator
    {
        private readonly IServiceProvider _serviceProvider;

        public DefaultControllerActivator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public object Create(ActionContext actionContext)
        {
            return _serviceProvider.GetService(actionContext.ControllerType);
        }

        public object Release(ActionContext actionContext, object controller)
        {
            // 
            return null;
        }
    }
}