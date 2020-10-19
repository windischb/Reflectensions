﻿using System;
using System.Collections.Generic;

namespace Reflectensions
{
    public interface IJson
    {
        
        string ToJson(object @object, bool formatted = false);
        string Beautify(string json);
        string Minify(string json);
        T ToObject<T>(string json);
        T ToObject<T>(object @object);
        object ToObject(string json, Type type);
        object ToObject(object @object, Type type);
        Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(object data);
        Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(string json);
        Dictionary<string, object> ToDictionary(object data);
        Dictionary<string, object> ToDictionary(string json);
        Dictionary<string, string> Flatten(object @object);

    }
}
