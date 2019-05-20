using Chromely.Core.RestfulService;
using System;

namespace Chromely.Mvc
{
    public class DefaultRouteResolver : IRouteResolver
    {
        private readonly RouteCache _routeCache = new RouteCache();
        private readonly ConventionalRouting _conventionalRouting;

        public DefaultRouteResolver(IControllerFactory controllerFactory, RouteCollection routeCollection)
        {
            _conventionalRouting = new ConventionalRouting(controllerFactory, routeCollection);
        }

        public ActionContext Resolve(RequestContext requestContext)
        {
            // TODO: Improve routing
            try
            {
                if (_routeCache.TryGetCachedRoute(requestContext, out ActionContext actionContext))
                {
                    return actionContext;
                }

                actionContext = _conventionalRouting.Resolve(requestContext);

                if (actionContext == null)
                {
                    throw new RouteException($"A matching route could not be found for the request. Method: \"{requestContext.Method}\" Url: \"{requestContext.Url}\" ", requestContext);
                }

                _routeCache.AddRoute(requestContext, actionContext);

                return actionContext;
            }
            catch (Exception ex)
            {
                throw new RouteException(requestContext, ex);
            }
        }

    }
}
