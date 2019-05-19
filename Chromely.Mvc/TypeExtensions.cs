using System;
using System.Collections.Generic;

namespace Chromely.Mvc
{
    public static class TypeExtensions
    {
        public static object GetDefaultValue(this Type t)
        {
            if (t.IsValueType && Nullable.GetUnderlyingType(t) == null)
                return Activator.CreateInstance(t);
            else
                return null;
        }
        public static bool IsGenericList(this Type t)
        {
            return (t.IsGenericType && (t.GetGenericTypeDefinition() == typeof(List<>)));
        }
    }
}