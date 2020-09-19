namespace Chromely.Mvc.Attributes
{
    public class HttpPostAttribute : HttpVerbAttribute
    {
        public override string Method { get; } = "POST";

        public HttpPostAttribute()
        {

        }
        public HttpPostAttribute(string action) : base(action)
        {

        }
    }
}
