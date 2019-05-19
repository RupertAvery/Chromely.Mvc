using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Chromely.Mvc
{
    public class MvcAction 
    {
        private ActionContext _actionContext;
        private ControllerContext _controllerContext;

        public bool IsAsync { get; private set; }
 
        public ActionContext ActionContext { get { return _actionContext;  } }
        public ControllerContext ControllerContext { get { return _controllerContext; } }

        public MvcAction(ActionContext actionContext, ControllerContext controllerContext)
        {
            _actionContext = actionContext;
            _controllerContext = controllerContext;
            IsAsync =
                _actionContext.ActionMethod.ReturnType.IsGenericType &&
                _actionContext.ActionMethod.ReturnType.GetGenericTypeDefinition() == typeof(Task<>);
        }

        public object Invoke(object[] parameters)
        {
            return _actionContext.ActionMethod.Invoke(_controllerContext.ControllerInstance, parameters);
        }
    }
}