using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Tests
{
    public static class StringExtensions
    {
        public static object ToExpandoObject(this String json)
        {
            var expConverter = new ExpandoObjectConverter();
            return JsonConvert.DeserializeObject<ExpandoObject>(json, expConverter);
        }

        public static object ToExpandoObjectList(this String json)
        {
            var expConverter = new ExpandoObjectConverter();
            return JsonConvert.DeserializeObject<List<ExpandoObject>>(json, expConverter);
        }

        public static object ToJsonElement(this String json)
        {
            return System.Text.Json.JsonDocument.Parse(json).RootElement;
        }


    }
}