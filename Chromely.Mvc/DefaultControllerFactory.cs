using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Chromely.Mvc
{
    /// <summary>
    /// A Controller Factory that uses MVC convention to register Controllers 
    /// </summary>
    public class DefaultControllerFactory : IControllerFactory
    {
        private static Regex ControllerRegex = new Regex("(.*)Controller$", RegexOptions.IgnoreCase);
        private Dictionary<string, Type> _controllers = new Dictionary<string, Type>();

        public DefaultControllerFactory()
        {
        }

        public IEnumerable<Type> Controllers
        {
            get
            {
                return _controllers.Values;
            }
        }

        public Type GetController(string controller)
        {
            if (_controllers.TryGetValue(controller.ToLowerInvariant(), out Type existingType))
            {
                return existingType;
            }
            else
            {
                throw new ControllerNotFoundException(controller);
            }
        }

        public void Add(string key, Type type)
        {
            if (_controllers.TryGetValue(key, out Type existingType))
            {
                throw new Exception($"Controller \"{existingType.Name}\" has already been registered with key \"{key}\"");
            }
            else
            {
                _controllers.Add(key, type);
            }
        }

        public void Add(Type type)
        {
            string key = type.Name;

            var match = ControllerRegex.Match(type.Name);

            if (match.Success)
            {
                key = match.Groups[1].Value;
            }

            key = key.ToLowerInvariant();

            Add(key, type);
        }

    }
}