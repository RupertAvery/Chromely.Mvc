using System.Collections.Generic;

namespace Chromely.Mvc
{
    public class RouteCache
    {
        private Dictionary<string, ActionContext> _routeCache = new Dictionary<string, ActionContext>();

        public bool TryGetCachedRoute(RequestContext requestContext, out ActionContext actionContext)
        {
            actionContext = null;

            var result = _routeCache.TryGetValue(GetRequestContextKey(requestContext),  out ActionContext existingActionContext);

            if (result)
            {
                actionContext = new ActionContext()
                {
                    ActionMethod = existingActionContext.ActionMethod,
                    ControllerType = existingActionContext.ControllerType,
                    Request = requestContext
                };
            }

            return result;
        }

        public void AddRoute(RequestContext requestContext, ActionContext actionContext)
        {
            var cachedActionContext = new ActionContext()
            {
                ActionMethod = actionContext.ActionMethod,
                ControllerType = actionContext.ControllerType
            };

            var key = GetRequestContextKey(requestContext);

            _routeCache.Add(key, cachedActionContext);
        }

        private string GetRequestContextKey(RequestContext requestContext)
        {
            return (requestContext.Method + ":" + requestContext.Url).ToLowerInvariant();
        }
    }
}