namespace Chromely.Mvc
{
    public interface IRouteResolver
    {
        ActionContext Resolve(RequestContext requestContext);
    }
}
