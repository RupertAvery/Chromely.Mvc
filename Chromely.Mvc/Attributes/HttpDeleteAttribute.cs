namespace Chromely.Mvc.Attributes
{
    public class HttpDeleteAttribute : HttpVerbAttribute
    {
        public override string Method { get; } = Methods.Delete;

        public HttpDeleteAttribute()
        {

        }
        public HttpDeleteAttribute(string action) : base(action)
        {

        }
    }
}
