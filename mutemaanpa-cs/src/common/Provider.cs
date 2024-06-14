namespace Mutemaanpa;

using System;
using System.Collections.Generic;
using Godot;

public class Provider
{
    private readonly Dictionary<Type, object> _deps = [];

    public static T Of<T>(Node node) where T : class => node switch
    {
        IProvider m => m.GetProvider()._deps.TryGetValue(typeof(T), out var value)
                    ? (T)value
                    : m is Main
                        ? throw new Exception("dependency not satisfied")
                        : Of<T>(node.GetParent()),
        _ => Of<T>(node.GetParent())
    };

    public void Add<T>(object dep)
    {
        _deps.Add(typeof(T), dep);
    }
}

public interface IProvider
{
    Provider GetProvider();
}
