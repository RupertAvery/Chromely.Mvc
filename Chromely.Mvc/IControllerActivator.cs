namespace Chromely.Mvc
{
    public interface IControllerActivator
    {
        object Create(ActionContext actionContext);
        object Release(ActionContext actionContext, object controller);
    }
}