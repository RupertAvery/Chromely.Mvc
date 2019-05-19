using System;
using System.Collections.Generic;

namespace Chromely.Mvc
{
    public class DefaultModelBinder : IModelBinder
    {
        public object BindToModel(Type type, object value)
        {
            if (value.GetType().IsGenericList())
            {
                return BindToArray(type, value);
            }
            else
            {
                return BindToObject(type, (IDictionary<string, object>)value);
            }
        }

        public object BindToArray(Type arrayType, object value)
        {
            if (!arrayType.IsGenericType)
            {
                throw new Exception("Parameter is not a generic type");
            }

            var elementType = arrayType.GetGenericArguments()[0];

            var listType = typeof(List<>);
            var constructedListType = listType.MakeGenericType(elementType);

            var instance = Activator.CreateInstance(constructedListType);

            var addMethod = constructedListType.GetMethod("Add");

            foreach (var element in (IEnumerable<object>)value)
            {
                addMethod.Invoke(instance, new[] { BindToObject(elementType, (IDictionary<string, object>)element) });
            }

            return instance;
        }

        public object BindToObject(Type type, IDictionary<string, object> value)
        {
            var properties = type.GetProperties();
            var boundObject = Activator.CreateInstance(type);
            foreach (var property in properties)
            {
                var boundValue = GetBoundValue(property.PropertyType, property.Name, value);
                property.SetValue(boundObject, boundValue);
            }
            return boundObject;
        }

        public object GetBoundValue(Type type, string name, IDictionary<string, object> valueLookup)
        {
            object value = type.GetDefaultValue();

            foreach (var keyvaluepair in valueLookup)
            {
                if (keyvaluepair.Key.ToLowerInvariant() == name.ToLowerInvariant())
                {

                    try
                    {
                        // DateTime
                        if (type == typeof(DateTime))
                        {
                            value = DateTime.Parse((string)keyvaluepair.Value);
                        }
                        // Numerics
                        else if (type == typeof(byte))
                        {
                            if (keyvaluepair.Value.GetType() == typeof(string))
                            {
                                value = byte.Parse((string)keyvaluepair.Value);
                            }
                            else
                            {
                                value = (byte)keyvaluepair.Value;
                            }
                        }
                        else if (type == typeof(short))
                        {
                            if (keyvaluepair.Value.GetType() == typeof(string))
                            {
                                value = short.Parse((string)keyvaluepair.Value);
                            }
                            else
                            {
                                value = (short)keyvaluepair.Value;
                            }
                        }
                        else if (type == typeof(int))
                        {
                            if (keyvaluepair.Value.GetType() == typeof(string))
                            {
                                value = int.Parse((string)keyvaluepair.Value);
                            }
                            if (keyvaluepair.Value.GetType() == typeof(long))
                            {
                                value = (int)(long)keyvaluepair.Value;
                            }
                            else
                            {
                                value = (int)keyvaluepair.Value;
                            }
                        }
                        else if (type == typeof(long))
                        {
                            if (keyvaluepair.Value.GetType() == typeof(string))
                            {
                                value = long.Parse((string)keyvaluepair.Value);
                            }
                            else
                            {
                                value = (long)keyvaluepair.Value;
                            }
                        }
                        else if (type == typeof(float))
                        {
                            if (keyvaluepair.Value.GetType() == typeof(string))
                            {
                                value = float.Parse((string)keyvaluepair.Value);
                            }
                            else
                            {
                                value = (float)keyvaluepair.Value;
                            }
                        }
                        else if (type == typeof(double))
                        {
                            if (keyvaluepair.Value.GetType() == typeof(string))
                            {
                                value = double.Parse((string)keyvaluepair.Value);
                            }
                            else
                            {
                                value = (double)keyvaluepair.Value;
                            }
                        }
                        else if (type == typeof(string))
                        {
                            if (keyvaluepair.Value.GetType() == typeof(string))
                            {
                                value = (string)keyvaluepair.Value;
                            }
                            else
                            {
                                value = keyvaluepair.Value.ToString();
                            }
                        }
                        else
                        {
                            value = BindToModel(type, keyvaluepair.Value);
                        }
                    }
                    catch
                    {
                        // log and ignore binding errors
                    }
                    break;
                }
            }
            return value;
        }

    }
}