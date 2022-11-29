// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc;
using System;
using System.Text.Json;

namespace DotBPE.Gateway.Tests.TestObjects
{
    public class DefaultJsonParser : IJsonParser
    {
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
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
            return JsonSerializer.Serialize(item, _jsonSerializerOptions);
        }

        public string ToJson<T>(T item) where T : class
        {
            return JsonSerializer.Serialize<object>(item, _jsonSerializerOptions);
        }

        public object FromJson(string json, Type type)
        {
            return JsonSerializer.Deserialize(json, type, _jsonSerializerOptions);
        }

        public T FromJson<T>(string json) where T : class
        {
            return JsonSerializer.Deserialize<T>(json, _jsonSerializerOptions);
        }

    }
}
