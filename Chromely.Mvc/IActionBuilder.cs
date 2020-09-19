namespace Chromely.Mvc
{
    public interface IActionBuilder
    {
        MvcAction BuildAction(RequestContext requestContext);
    }
}