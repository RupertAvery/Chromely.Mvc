using System;

namespace Chromely.Mvc.Attributes
{
    public abstract class HttpVerbAttribute : Attribute
    {
        public abstract string Method { get; }
        public string Action { get; }

        protected HttpVerbAttribute()
        {
        }

        protected HttpVerbAttribute(string action)
        {
            Action = action;
        }
    }
}
