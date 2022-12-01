// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc;
using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace DotBPE.Extra
{
    public class TextJsonParser : IJsonParser
    {
        private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
        {
#if NET5_0_OR_GREATER
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
#else
            IgnoreNullValues = true,
#endif
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            IgnoreReadOnlyProperties = false,
            AllowTrailingCommas = false
        };

        public string ToJson(object item)
        {
            return JsonSerializer.Serialize(item, JsonSerializerOptions);
        }

        public string ToJson<T>(T item) where T : class
        {
            return JsonSerializer.Serialize<object>(item, JsonSerializerOptions);
        }

        public object FromJson(string json, Type type)
        {
            return JsonSerializer.Deserialize(json, type, JsonSerializerOptions);
        }

        public T FromJson<T>(string json) where T : class
        {
            return JsonSerializer.Deserialize<T>(json, JsonSerializerOptions);
        }

    }
}
