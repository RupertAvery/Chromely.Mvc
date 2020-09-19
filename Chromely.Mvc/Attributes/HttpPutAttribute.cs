namespace Chromely.Mvc.Attributes
{
    public class HttpPutAttribute : HttpVerbAttribute
    {
        public override string Method { get; } = "PUT";

        public HttpPutAttribute()
        {

        }
        public HttpPutAttribute(string action) : base(action)
        {

        }
    }
}
