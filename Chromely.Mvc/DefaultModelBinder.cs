using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Chromely.Core.Network;

namespace Chromely.Mvc
{
    public class DefaultModelBinder : IModelBinder
    {
        public object BindToModel(Type type, object value)
        {
            if (value == null)
            {
                return null;
            }
            else if (value.GetType().IsGenericList())
            {
                return BindToArray(type, value);
            }
            return null;
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
                addMethod.Invoke(instance, new[] { BindToModel(elementType, (IDictionary<string, object>)element) });
            }

            return instance;
        }

        /// <summary>
        /// Creates and instance of the type and binds the dictionary values to matching properties
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public object BindToModel(Type type, JsonElement value)
        {
            var isEnumerable = typeof(IEnumerable).IsAssignableFrom(type);
            Type elementType = null;

            if (isEnumerable)
            {
                elementType = type.GetGenericArguments()[0];
                type = typeof(List<>).MakeGenericType(elementType);
            }

            object boundObject = type.GetDefaultValue();

            if (isEnumerable && value.ValueKind == JsonValueKind.Array)
            {
                boundObject = Activator.CreateInstance(type);

                foreach (var element in value.EnumerateArray())
                {
                    var boundValue = GetBoundValue(elementType, element);
                    ((IList)boundObject).Add(boundValue);
                }
            }
            else
            {
                boundObject = GetBoundValue(type, value);
            }

            return boundObject;
        }

        /// <summary>
        /// Creates and instance of the type and binds the dictionary values to matching properties
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public object BindToModel(Type type, IDictionary<string, object> value)
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

        /// <summary>
        /// Helper function to handle conversion
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="parseFunc"></param>
        /// <param name="underlyingType"></param>
        /// <returns></returns>
        public object GetBoundValueType<T>(object value, Func<string, T> parseFunc, Type underlyingType)
        {
            // Handle nulls
            if (value == null)
            {
                return null;
            }

            // Try to parse if value is a string
            if (value.GetType() == typeof(string))
            {
                value = parseFunc((string)value);
            }
            else
            {
                // If it was a nullable type, try to cast
                if (underlyingType != null)
                {
                    value = Convert.ChangeType(value, underlyingType);
                }

                // otherwise return as-is
            }

            return value;
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
                        var underlyingType = Nullable.GetUnderlyingType(type);
                        if (underlyingType != null)
                        {
                            type = underlyingType;
                        }

                        // DateTime
                        if (type == typeof(DateTime))
                        {
                            value = GetBoundValueType<DateTime>(keyvaluepair.Value, DateTime.Parse, underlyingType);
                            //if (keyvaluepair.Value == null)
                            //{
                            //    value = null;
                            //    break;
                            //}
                            //value = DateTime.Parse((string)keyvaluepair.Value);
                        }
                        else
                        {
                            if (type == typeof(byte))
                            {
                                value = GetBoundValueType<byte>(keyvaluepair.Value, byte.Parse, underlyingType);
                            }
                            else if (type == typeof(short))
                            {
                                value = GetBoundValueType<short>(keyvaluepair.Value, short.Parse, underlyingType);
                            }
                            else if (type == typeof(int))
                            {
                                if (keyvaluepair.Value == null)
                                {
                                    value = null;
                                    break;
                                }

                                if (keyvaluepair.Value.GetType() == typeof(string))
                                {
                                    value = int.Parse((string)keyvaluepair.Value);
                                }

                                // in case the data is deserialized as a long type, but field is int
                                if (keyvaluepair.Value.GetType() == typeof(long))
                                {
                                    if (underlyingType != null)
                                    {
                                        value = (int)Convert.ChangeType(keyvaluepair.Value, underlyingType);
                                    }
                                    else
                                    {
                                        value = (int)(long)keyvaluepair.Value;
                                    }
                                }
                                else
                                {
                                    if (underlyingType != null)
                                    {
                                        value = Convert.ChangeType(keyvaluepair.Value, underlyingType);
                                    }
                                    else
                                    {
                                        value = (int)keyvaluepair.Value;
                                    }
                                }
                            }
                            else if (type == typeof(long))
                            {
                                value = GetBoundValueType<long>(keyvaluepair.Value, long.Parse, underlyingType);
                            }
                            else if (type == typeof(float))
                            {
                                value = GetBoundValueType<float>(keyvaluepair.Value, float.Parse, underlyingType);
                            }
                            else if (type == typeof(double))
                            {
                                value = GetBoundValueType<double>(keyvaluepair.Value, double.Parse, underlyingType);
                            }
                            else if (type == typeof(string))
                            {
                                if (keyvaluepair.Value == null)
                                {
                                    value = null;
                                    break;
                                }

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

        public object GetBoundValue(Type type, JsonElement valueLookup)
        {
            object value = type.GetDefaultValue();

            if (valueLookup.ValueKind == JsonValueKind.Object)
            {
                value = Activator.CreateInstance(type);
            }

            var properties = type.GetProperties().ToDictionary(x => x.Name.ToLowerInvariant(), x => x);

            foreach (var keyvaluepair in valueLookup.EnumerateObject())
            {
                object propertyValue = null;
                if (properties.TryGetValue(keyvaluepair.Name.ToLowerInvariant(), out PropertyInfo property))
                {
                    var propertyType = property.PropertyType;
                    try
                    {
                        var underlyingType = Nullable.GetUnderlyingType(propertyType);
                        if (underlyingType != null)
                        {
                            propertyType = underlyingType;
                        }

                        // DateTime
                        if (propertyType == typeof(DateTime))
                        {
                            propertyValue = keyvaluepair.Value.GetDateTime();
                        }
                        else
                        {
                            if (propertyType == typeof(byte))
                            {
                                propertyValue = GetBoundValueType<byte>(keyvaluepair.Value, byte.Parse, underlyingType);
                            }
                            else if (propertyType == typeof(short))
                            {
                                propertyValue = GetBoundValueType<short>(keyvaluepair.Value, short.Parse, underlyingType);
                            }
                            else if (propertyType == typeof(int))
                            {
                                if (keyvaluepair.Value.ValueKind == JsonValueKind.Null)
                                {
                                    propertyValue = null;
                                    break;
                                }

                                if (keyvaluepair.Value.ValueKind == JsonValueKind.String)
                                {
                                    propertyValue = int.Parse((string)keyvaluepair.Value.GetString());
                                }

                                // in case the data is deserialized as a long type, but field is int
                                if (keyvaluepair.Value.GetType() == typeof(long))
                                {
                                    if (underlyingType != null)
                                    {
                                        propertyValue = (int)Convert.ChangeType(keyvaluepair.Value, underlyingType);
                                    }
                                    else
                                    {
                                        propertyValue = (int)(long)keyvaluepair.Value.GetInt64();
                                    }
                                }
                                else
                                {
                                    propertyValue = (int)keyvaluepair.Value.GetInt32();

                                    //if (underlyingType != null)
                                    //{
                                    //    propertyValue = Convert.ChangeType(keyvaluepair.Value, underlyingType);
                                    //}
                                    //else
                                    //{
                                    //}
                                }
                            }
                            else if (propertyType == typeof(long))
                            {
                                propertyValue = keyvaluepair.Value.GetInt64();
                            }
                            else if (propertyType == typeof(float))
                            {
                                propertyValue = keyvaluepair.Value.GetSingle();
                            }
                            else if (propertyType == typeof(double))
                            {
                                propertyValue = keyvaluepair.Value.GetDouble();
                            }
                            else if (propertyType == typeof(string))
                            {
                                if (keyvaluepair.Value.ValueKind == JsonValueKind.Null)
                                {
                                    propertyValue = null;
                                }
                                else
                                {
                                    propertyValue = (string)keyvaluepair.Value.GetString();
                                }

                                //if (keyvaluepair.Value.ValueKind == JsonValueKind.String)
                                //{
                                //}
                                //else
                                //{
                                //    propertyValue = keyvaluepair.Value.ToString();
                                //}
                            }
                            else
                            {
                                propertyValue = BindToModel(propertyType, keyvaluepair.Value);
                            }
                        }
                    }
                    catch
                    {
                        // log and ignore binding errors
                    }
                    property.SetValue(value, propertyValue);

                }
            }
            return value;
        }

    }
}