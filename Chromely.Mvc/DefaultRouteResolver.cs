using System;
using Microsoft.Extensions.Logging;

namespace Chromely.Mvc
{
    public class DefaultRouteResolver : IRouteResolver
    {
        private readonly ILogger _logger;
        private readonly RouteCache _routeCache = new RouteCache();
        private readonly ConventionalRouting _conventionalRouting;

        public DefaultRouteResolver(IControllerFactory controllerFactory, RouteCollection routeCollection, ILoggerProvider loggerProvider)
        {
            _logger = loggerProvider.CreateLogger("RouterReolver");
            _conventionalRouting = new ConventionalRouting(controllerFactory, routeCollection);
        }

        public ActionContext Resolve(RequestContext requestContext)
        {

            // TODO: Improve routing
            try
            {
                if (_routeCache.TryGetCachedRoute(requestContext, out ActionContext actionContext))
                {
                    _logger.Log(LogLevel.Trace, $"Cached route returned for {requestContext.Url}");
                    return actionContext;
                }

                actionContext = _conventionalRouting.Resolve(requestContext);

                if (actionContext == null)
                {
                    throw new RouteException($"A matching route could not be found for the request. Method: \"{requestContext.Method}\" Url: \"{requestContext.Url}\" ", requestContext);
                }

                _routeCache.AddRoute(requestContext, actionContext);

                _logger.Log(LogLevel.Trace, $"New route returned for {requestContext.Url}");
                return actionContext;
            }
            catch (RouteException ex)
            {
                _logger.Log(LogLevel.Error, ex, ex.Message, requestContext.Url);
                throw;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex, ex.Message, requestContext.Url);
                throw new RouteException(requestContext, ex);
            }
        }

    }
}
