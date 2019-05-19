namespace Chromely.Mvc
{
    public class Route
    {
        public string Key { get; }
        public string RoutePath { get; }
        public object Defaults { get; }

        public Route(string key, string route)
        {
            Key = key;
            RoutePath = route;
        }

        public Route(string key, string route, object defaults)
        {
            Key = key;
            RoutePath = route;
            Defaults = defaults;
        }
    }
}
