
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mutemaanpa;
public class HitEvent(string victim, float damage) : EventArgs
{
    public string Victim { get; } = victim;
    public float Damage { get; } = damage;
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
    private static readonly Dictionary<Type, List<(Delegate, bool)>> eventHandlers = [];
    private static readonly object Lock = new();

    /// <summary>
    /// Subscribes an game-wide event
    /// 
    /// NOTE: If you subscribe some events, remember to unsubscribe it according to your object's
    /// lifetime, otherwise segmentation fault may happens.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="handler"></param> <summary>
    /// 
    /// </summary>
    /// <param name="handler"></param>
    /// <typeparam name="T"></typeparam>
    public static void Subscribe<T>(Action<T> handler) where T : EventArgs
    {
        Type type = typeof(T);
        lock (Lock)
        {
            if (!eventHandlers.TryGetValue(type, out List<(Delegate, bool)>? value))
            {
                value ??= [];
                eventHandlers[type] = value;
            }
            value?.Add((handler, true));
        }
    }

    private static List<(Delegate, bool)> Vacuum(List<(Delegate, bool)> subscribers)
    {
        return subscribers.Where(f => f.Item2).ToList();
    }

    public static void Unsubscribe<T>(Action<T> handler) where T : EventArgs
    {
        Type type = typeof(T);
        lock (Lock)
        {
            if (eventHandlers.TryGetValue(type, out List<(Delegate, bool)>? value))
            {
                var idx = value.FindIndex(f => f.Item1.Equals(handler));
                var item = value[idx];
                item.Item2 = false;
                value[idx] = item;
            }
        }
    }

    public static void Publish<T>(T args) where T : EventArgs
    {
        Type type = typeof(T);
        lock (Lock)
        {
            if (eventHandlers.TryGetValue(type, out List<(Delegate, bool)>? value))
            {
                foreach (var (handler, _) in value)
                {
                    ((Action<T>)handler)(args);
                }
                eventHandlers[type] = Vacuum(eventHandlers[type]);
            }
        }
    }
}

