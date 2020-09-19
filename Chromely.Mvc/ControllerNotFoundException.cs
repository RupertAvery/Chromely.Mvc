using System;

namespace Chromely.Mvc
{
    public class ControllerNotFoundException : Exception
    {
        public string ControllerName { get; set; }
        public ControllerNotFoundException(string name) : base(GetMessage(name))
        {
            ControllerName = name;
        }

        private static string GetMessage(string name)
        {
            return $"Controller with name \"{name}\" was not found.";
        }
    }
}
