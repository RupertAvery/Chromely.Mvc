using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Chromely.Mvc
{
    public static class AssemblyExtensions
    {

        /// <summary>
        /// Scans the specified assembly for types are assignable from <see cref="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetTypes<T>(this Assembly assembly)
        {
            var types = from type in assembly.GetTypes()
                where typeof(T).IsAssignableFrom(type)
                select type;

            foreach (var type in types)
            {
                yield return type;
            }
        }
    }
}