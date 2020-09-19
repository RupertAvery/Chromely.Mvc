using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Chromely.Mvc
{
    public class MvcAction
    {
        public bool IsAsync { get; private set; }
        public ActionContext ActionContext { get; private set; }
        public ControllerContext ControllerContext { get; private set; }

        public MvcAction(ActionContext actionContext, ControllerContext controllerContext)
        {
            ActionContext = actionContext;
            ControllerContext = controllerContext;
            IsAsync =
                ActionContext.ActionMethod.ReturnType.IsGenericType &&
                ActionContext.ActionMethod.ReturnType.GetGenericTypeDefinition() == typeof(Task<>) || ActionContext.ActionMethod.ReturnType == typeof(Task);
        }

        public async Task<object> InvokeAsync(object[] parameters)
        {
            var asyncObject = (Task)ActionContext.ActionMethod.Invoke(ControllerContext.ControllerInstance, parameters);

            await asyncObject.ConfigureAwait(false);

            var taskType = asyncObject.GetType();
            var resultProperty = taskType.GetProperty("Result");

            return resultProperty.GetValue(asyncObject);
        }

        public object Invoke(object[] parameters)
        {
            return ActionContext.ActionMethod.Invoke(ControllerContext.ControllerInstance, parameters);
        }
    }
}