using Chromely.Core;

namespace Chromely.Mvc
{

    public class DefaultActionBuilder : IActionBuilder
    {
        private readonly IRouteResolver _actionResolver;
        private readonly IControllerActivator _controllerActivator;

        public DefaultActionBuilder(IRouteResolver actionResolver, IControllerActivator controllerActivator)
        {
            _actionResolver = actionResolver;
            _controllerActivator = controllerActivator;
        }

        public MvcAction BuildAction(RequestContext requestContext)
        {
            var actionContext = _actionResolver.Resolve(requestContext);

            var controllerInstance = (Controller)_controllerActivator.Create(actionContext);
            var controllerContext = new ControllerContext(controllerInstance);

            controllerInstance.Request = requestContext;

            return new MvcAction(actionContext, controllerContext);
        }
    }
}