using System;
using System.Collections.Generic;
using System.Reflection;

namespace Chromely.Mvc
{
    public interface IControllerFactory
    {
        Type GetController(string controller);
        IEnumerable<Type> Controllers { get; }
        void Add(string key, Type type);
        void Add(Type type);
        IEnumerable<Type> ScanAssembly(Assembly assembly);
    }
}
