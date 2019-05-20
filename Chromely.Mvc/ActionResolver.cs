using Chromely.Core.RestfulService;
using Chromely.Mvc.Attributes;
using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Chromely.Mvc
{
    public class RouteResolver : IRouteResolver
    {
        private RouteCollection _routeCollection;

        private IControllerFactory _controllerFactory;

        public RouteResolver(IControllerFactory controllerFactory, RouteCollection routeCollection)
        {
            _controllerFactory = controllerFactory;
            _routeCollection = routeCollection;
        }

        public ActionContext Resolve(RequestContext requestContext)
        {

            foreach (Route route in _routeCollection)
            {
                var routePattern = route.RoutePath
                    .Replace("/{controller}", "(?:/(?<controller>[a-zA-Z0-9-.]+)?)?")
                    .Replace("/{action}", "(?:/(?<action>[a-zA-Z0-9-.]+)?)?");

                var routeRegex = new Regex(routePattern, RegexOptions.IgnoreCase);

                var match = routeRegex.Match(requestContext.Url);

                if (match.Success)
                {
                    var controller = match.Groups["controller"].Value;
                    var controllerType = _controllerFactory.GetController(controller);

                    var action = "";

                    if (match.Groups["action"].Success)
                    {
                        action = match.Groups["action"].Value;
                    }
                    else
                    {
                        action = "index";
                    }

                    var methodInfo = GetMatchingAction(requestContext.Method, controllerType, action);

                    if(methodInfo != null)
                    {
                        return new ActionContext()
                        {
                            ActionMethod = methodInfo,
                            ControllerType = controllerType,
                            Request = requestContext
                        };
                    }
                }
            }

            throw new RouteException(requestContext);
        }


        private MethodInfo GetMatchingAction(string method, Type controllerType, string action)
        {
            foreach (var methodInfo in controllerType.GetMethods())
            {
                foreach (var attribute in methodInfo.GetCustomAttributes())
                {
                    if (typeof(HttpVerbAttribute).IsAssignableFrom(attribute.GetType()))
                    {
                        var verbAttribute = ((HttpVerbAttribute)attribute);

                        if (verbAttribute.Method.ToUpperInvariant() == method.ToUpperInvariant()
                            && (
                            action.ToLowerInvariant() == methodInfo.Name.ToLowerInvariant()
                            || (verbAttribute.Action != null && action.ToLowerInvariant() == verbAttribute.Action.ToLowerInvariant())
                            ))
                        {
                            return methodInfo;
                        }
                    }
                }
            }

            return null;
        }
    }
}
