namespace SolTK;
using System;
using System.Collections.Generic;
using System.Linq;


public static class EventBus
{
    // Dictionary of Type -> List of (Priority, Handler)
    private static readonly Dictionary<Type, List<(int Priority, Delegate Handler)>> _globalHandlers = [];
    private static readonly Dictionary<Type, List<(int Priority, Delegate Handler)>> _localHandlers = [];

    // --- GLOBAL EVENTS ---
    public static void Subscribe<T>(Action<T> handler, int priority = 0)
    {
        var list = _globalHandlers.GetOrAdd(typeof(T));
        list.Add((priority, handler));
        // Sort ascending so lowest priority runs first (or descending, based on your preference)
        list.Sort((a, b) => a.Item1.CompareTo(b.Item1)); 
    }

    public static void Raise<T>(T ev)
    {
        if (_globalHandlers.TryGetValue(typeof(T), out var list))
        {
            // Iterate backwards to safely allow unsubscribe during iteration if needed
            for (int i = list.Count - 1; i >= 0; i--)
            {
                ((Action<T>)list[i].Handler).Invoke(ev);
            }
        }
    }

    // --- DIRECTED / LOCAL EVENTS ---
    public static void SubscribeLocal<T>(Action<Entity, T> handler, int priority = 0)
    {
        var list = _localHandlers.GetOrAdd(typeof(T));
        list.Add((priority, handler));
        list.Sort((a, b) => a.Item1.CompareTo(b.Item1));
    }

    public static void RaiseLocal<T>(Entity target, T ev)
    {
        if (_localHandlers.TryGetValue(typeof(T), out var list))
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                ((Action<Entity, T>)list[i].Handler).Invoke(target, ev);
            }
        }
    }
    
    // Helper to get or create the list
    private static List<(int, Delegate)> GetOrAdd(this Dictionary<Type, List<(int, Delegate)>> dict, Type type)
    {
        if (!dict.TryGetValue(type, out var list))
        {
            list = [];
            dict[type] = list;
        }
        return list;
    }
}