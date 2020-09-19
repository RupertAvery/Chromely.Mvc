namespace Chromely.Mvc.Attributes
{
    public class HttpDeleteAttribute : HttpVerbAttribute
    {
        public override string Method { get; } = "DELETE";

        public HttpDeleteAttribute()
        {

        }
        public HttpDeleteAttribute(string action) : base(action)
        {

        }
    }
}
