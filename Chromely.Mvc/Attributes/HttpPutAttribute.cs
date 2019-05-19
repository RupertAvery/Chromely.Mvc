namespace Chromely.Mvc.Attributes
{
    public class HttpPutAttribute : HttpVerbAttribute
    {
        public override string Method { get; } = Methods.Put;

        public HttpPutAttribute()
        {

        }
        public HttpPutAttribute(string action) : base(action)
        {

        }
    }
}
