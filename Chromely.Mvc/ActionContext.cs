using System;
using System.Reflection;

namespace Chromely.Mvc
{
    public class ActionContext
    {
        public Type ControllerType { get; set; }
        public MethodInfo ActionMethod { get; set; }
        public RequestContext Request { get; set; }
    }
}
