using System.Collections.Generic;

namespace Chromely.Mvc
{
    public class RequestContext
    {
        public string Method { get; set; }
        public string Url { get; set; }
        public IEnumerable<KeyValuePair<string, string>> QueryParameters { get; set; }
        public object PostData { get; set; }
    }
}