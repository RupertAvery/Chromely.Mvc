using System;
using System.Reflection;
using Chromely.Mvc.Attributes;

namespace Chromely.Mvc
{
    public class AttributeRouting
    {
        private IControllerFactory _controllerFactory;

        private Type GetMatchingController(string url)
        {
            foreach (var controllerType in _controllerFactory.Controllers)
            {
                foreach (var attribute in controllerType.GetCustomAttributes())
                {
                    if (attribute is RouteAttribute routeAttribute)
                    {

                        if (false)
                        {
                            return controllerType;
                        }
                    }
                }
            }

            return null;
        }
    }
}