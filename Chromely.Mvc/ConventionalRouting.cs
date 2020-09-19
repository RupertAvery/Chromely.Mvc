using System;
using System.Reflection;
using System.Text.RegularExpressions;
using Chromely.Core.Network;
using Chromely.Mvc.Attributes;

namespace Chromely.Mvc
{
    public class ConventionalRouting
    {
        private readonly RouteCollection _routeCollection;
        private readonly IControllerFactory _controllerFactory;

        public ConventionalRouting(IControllerFactory controllerFactory, RouteCollection routeCollection)
        {
            _routeCollection = routeCollection;
            _controllerFactory = controllerFactory;
        }

        public ActionContext Resolve(RequestContext requestContext)
        {
            foreach (Route route in _routeCollection)
            {
                // TODO: Improve tokenizing
                var routePattern = route.RoutePath
                    .Replace("/{controller}", "(?:/(?<controller>[a-zA-Z0-9-.]+)?)?")
                    .Replace("/{action}", "(?:/(?<action>[a-zA-Z0-9-.]+)?)?");

                var routeRegex = new Regex(routePattern, RegexOptions.IgnoreCase);

                var match = routeRegex.Match(requestContext.Url);

                if (match.Success)
                {
                    var controllerName = match.Groups["controller"].Value;

                    var controllerType = _controllerFactory.GetController(controllerName);

                    string[] actions = { };

                    if (match.Groups["action"].Success)
                    {
                        actions = new[] { match.Groups["action"].Value };
                    }
                    else
                    {
                        switch (requestContext.Method)
                        {
                            case Method.GET:
                                actions = new[] { "index", "get" };
                                break;
                            case Method.POST:
                                actions = new[] { "post" };
                                break;
                            case Method.PUT:
                                actions = new[] { "put" };
                                break;
                            case Method.DELETE:
                                actions = new[] { "delete" };
                                break;
                            case Method.OPTIONS:
                                actions = new[] { "options" };
                                break;
                            case Method.HEAD:
                                actions = new[] { "head" };
                                break;
                            case Method.MERGE:
                                actions = new[] { "merge" };
                                break;
                        }
                    }


                    foreach (var action in actions)
                    {
                        var methodInfo = GetMatchingAction(requestContext.Method, controllerType, action);

                        if (methodInfo != null)
                        {
                            var actionContext = new ActionContext()
                            {
                                ActionMethod = methodInfo,
                                ControllerType = controllerType,
                                Request = requestContext
                            };

                            return actionContext;
                        }
                    }
                }
            }

            return null;
        }


        private MethodInfo GetMatchingAction(Method method, Type controllerType, string action)
        {
            foreach (var methodInfo in controllerType.GetMethods())
            {
                foreach (var attribute in methodInfo.GetCustomAttributes())
                {
                    if (attribute is HttpVerbAttribute verbAttribute)
                    {
                        if (string.Equals(verbAttribute.Method, Enum.GetName(typeof(Method), method), StringComparison.InvariantCultureIgnoreCase)
                            && (
                                string.Equals(action, methodInfo.Name, StringComparison.InvariantCultureIgnoreCase)
                                || (verbAttribute.Action != null && string.Equals(action, verbAttribute.Action, StringComparison.InvariantCultureIgnoreCase))
                            ))
                        {
                            return methodInfo;
                        }
                    }
                }
            }

            foreach (var methodInfo in controllerType.GetMethods())
            {
                if (string.Equals(methodInfo.Name, Enum.GetName(typeof(Method), method), StringComparison.InvariantCultureIgnoreCase))
                {
                    return methodInfo;
                }
            }

            return null;
        }

    }
}