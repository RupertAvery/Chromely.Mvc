namespace Chromely.Mvc.Attributes
{

    public class HttpGetAttribute : HttpVerbAttribute
    {
        public override string Method { get; } = Methods.Get;

        public HttpGetAttribute()
        {

        }
        public HttpGetAttribute(string action) : base(action)
        {

        }
    }
}
