using Chromely.Core.RestfulService;

namespace Chromely.Mvc
{
    public interface IActionBuilder
    {
        MvcAction BuildAction(RequestContext requestContext);
    }
}