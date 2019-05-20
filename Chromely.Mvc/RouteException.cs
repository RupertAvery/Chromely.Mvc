using System;

namespace Chromely.Mvc
{
    public class RouteException : Exception
    {
        public RequestContext RequestContext { get; set; }

        public RouteException(string message, RequestContext requestContext) : base(message)
        {
            RequestContext = requestContext;
        }


        public RouteException(RequestContext requestContext) : base(GetMessage(requestContext))
        {
            RequestContext = requestContext;
        }

        public RouteException(RequestContext requestContext, Exception innerException): base(GetMessage(requestContext, innerException.Message), innerException)
        {
            RequestContext = requestContext;
        }

        private static string GetMessage(RequestContext requestContext)
        {
            return $"An error occurred while resolving the route: Method: \"{requestContext.Method}\" Url: \"{requestContext.Url}\"";
        }

        private static string GetMessage(RequestContext requestContext, string message)
        {
            return $"An error occurred while resolving the route: Method: \"{requestContext.Method}\" Url: \"{requestContext.Url}\". Error: ${message}";
        }

    }
}
