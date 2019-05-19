using System.Collections;
using System.Collections.Generic;

namespace Chromely.Mvc
{
    public class RouteCollection : IEnumerable<Route>
    {
        List<Route> _routes = new List<Route>();

        public IEnumerator<Route> GetEnumerator()
        {
            return ((IEnumerable<Route>)_routes).GetEnumerator();
        }

        public void MapRoute(string key, string route)
        {
            _routes.Add(new Route(key, route));
        }

        public void MapRoute(string key, string route, object defaults)
        {
            _routes.Add(new Route(key, route, defaults));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<Route>)_routes).GetEnumerator();
        }
    }
}
