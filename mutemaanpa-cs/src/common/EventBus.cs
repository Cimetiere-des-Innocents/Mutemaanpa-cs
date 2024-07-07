namespace Mutemaanpa;

using System;
using System.Collections.Generic;


public class HitEvent(string victim, float damage) : EventArgs
{
    public string Victim { get; } = victim;
    public float Damage { get; } = damage;
}

public class DeadEvent(string victim) : EventArgs
{
    public string Victim { get; } = victim;
}

/// <summary>
/// EventBus broadcasts events from multiple publisher to multiple subscribers.
/// 
/// # Usage
/// 
/// 1. define a new event type here, it must extends EventArgs
/// 2. use EventBus.Publish to publish the event
/// 3. use EventBus.Subscribe to subscribe the event
/// 
/// Note that this class does not support event hierarchy or groups. We can see if it is needed.
/// </summary>
public static class EventBus
{
    private static readonly Dictionary<Type, List<Delegate>> eventHandlers = [];
    private static readonly object Lock = new();

    public static void Subscribe<T>(Action<T> handler) where T : EventArgs
    {
        Type type = typeof(T);
        lock (Lock)
        {
            if (!eventHandlers.TryGetValue(type, out List<Delegate>? value))
            {
                value ??= [];
                eventHandlers[type] = value;
            }
            value?.Add(handler);
        }
    }

    public static void Unsubscribe<T>(Action<T> handler) where T : EventArgs
    {
        Type type = typeof(T);
        lock (Lock)
        {
            if (eventHandlers.TryGetValue(type, out List<Delegate>? value))
            {
                value.Remove(handler);
                if (value.Count == 0)
                {
                    eventHandlers.Remove(type);
                }
            }
        }
    }

    public static void Publish<T>(T args) where T : EventArgs
    {
        Type type = typeof(T);
        lock (Lock)
        {
            if (eventHandlers.TryGetValue(type, out List<Delegate>? value))
            {
                foreach (var handler in value)
                {
                    ((Action<T>)handler)(args);
                }
            }
        }
    }
}

