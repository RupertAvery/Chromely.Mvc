namespace Chromely.Mvc
{
    public abstract class Controller
    {
        public ControllerContext ControllerContext { get; set; }
        public RequestContext RequestContext { get; set; }

    }
}