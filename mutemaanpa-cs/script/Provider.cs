namespace Mutemaanpa;

using System;
using System.Collections.Generic;
using Godot;

public partial class Provider
{
    private readonly Dictionary<Type, object> _deps = [];

    public static T Of<T>(Node node) where T : class => node switch
    {
        Main m => m.provider._deps.TryGetValue(typeof(T), out var value)
                    ? (T) value
                    : throw new Exception("dependency not satisfied"),
        _ => Of<T>(node.GetParent())
    };

    public void Add<T>(object dep)
    {
        _deps.Add(typeof(T), dep);
    }
}
