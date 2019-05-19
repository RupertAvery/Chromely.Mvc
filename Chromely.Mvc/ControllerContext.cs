namespace Chromely.Mvc
{

    public class ControllerContext
    {
        public object ControllerInstance { get; set; }

        public ControllerContext(object controllerInstance)
        {
            ControllerInstance = controllerInstance;
        }
    }
}