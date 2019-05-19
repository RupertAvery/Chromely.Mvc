using System;

namespace Chromely.Mvc
{
    public class RouteException : Exception
    {
        public RequestContext RequestContext { get; set; }

        public RouteException(RequestContext requestContext)
        {
            RequestContext = requestContext;
        }
    }
}
