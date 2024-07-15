using System;
using System.Collections.Generic;
using Godot;

namespace Mutemaanpa;

public static class Extension
{
    public static List<KinematicCollision3D> GetSlideCollisions(this CharacterBody3D body)
    {
        var collisionCount = body.GetSlideCollisionCount();
        List<KinematicCollision3D> nodes = [];
        for (int i = 0; i < collisionCount; i++)
        {
            nodes.Add(body.GetSlideCollision(i));
        }
        return nodes;
    }

    public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
    where TValue : new()
    {
        return GetOrCreate(dict, key, () => new TValue());
    }

    public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TValue> provider)
    {
        if (!dict.TryGetValue(key, out var val))
        {
            val = provider();
            dict.Add(key, val);
        }
        return val;
    }

    public static void InsertOrReplace<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
    {
        if (!dictionary.ContainsKey(key))
        {
            dictionary.Add(key, value);
        }
        else
        {
            dictionary[key] = value;
        }
    }
}
