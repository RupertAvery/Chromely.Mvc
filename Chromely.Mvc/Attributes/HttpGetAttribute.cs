namespace Chromely.Mvc.Attributes
{

    public class HttpGetAttribute : HttpVerbAttribute
    {
        public override string Method { get; } = "GET";

        public HttpGetAttribute()
        {

        }
        public HttpGetAttribute(string action) : base(action)
        {

        }
    }
}
