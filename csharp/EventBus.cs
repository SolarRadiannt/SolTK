namespace SolTK;

using System;
using System.Collections.Generic;

public static class EventBus
{
    public delegate void RefAction<T>(ref T ev);
    public delegate void LocalRefAction<T>(Entity target, ref T ev);

    // Per-event-type static storage. The CLR gives each closed generic
    // its own static fields, so this is effectively a compile-time
    // dictionary with zero lookup cost in the hot path.
    private static class Global<T>
    {
        public static readonly List<(int Priority, RefAction<T> Handler)> Subs = [];
        public static RefAction<T>[] Cache = Array.Empty<RefAction<T>>();
        public static bool Dirty = true;
    }

    private static class Local<T>
    {
        public static readonly List<(int Priority, LocalRefAction<T> Handler)> Subs = [];
        public static LocalRefAction<T>[] Cache = Array.Empty<LocalRefAction<T>>();
        public static bool Dirty = true;
    }

    // ------------------- GLOBAL EVENTS -------------------

    public static void Subscribe<T>(RefAction<T> handler, int priority = 0)
    {
        var subs = Global<T>.Subs;
        subs.Add((priority, handler));
        subs.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        Global<T>.Dirty = true;
    }

    public static void Unsubscribe<T>(RefAction<T> handler)
    {
        var subs = Global<T>.Subs;
        for (int i = subs.Count - 1; i >= 0; i--)
        {
            if (subs[i].Handler == handler)
                subs.RemoveAt(i);
        }
        Global<T>.Dirty = true;
    }

    public static void Raise<T>(ref T ev)
    {
        if (Global<T>.Dirty)
            RebuildGlobalCache<T>();

        var cache = Global<T>.Cache;
        for (int i = 0; i < cache.Length; i++)
            cache[i](ref ev);
    }

    private static void RebuildGlobalCache<T>()
    {
        var subs = Global<T>.Subs;
        var cache = new RefAction<T>[subs.Count];
        for (int i = 0; i < subs.Count; i++)
            cache[i] = subs[i].Handler;

        Global<T>.Cache = cache;
        Global<T>.Dirty = false;
    }

    // ------------------- LOCAL EVENTS -------------------

    public static void SubscribeLocal<T>(LocalRefAction<T> handler, int priority = 0)
    {
        var subs = Local<T>.Subs;
        subs.Add((priority, handler));
        subs.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        Local<T>.Dirty = true;
    }

    public static void UnsubscribeLocal<T>(LocalRefAction<T> handler)
    {
        var subs = Local<T>.Subs;
        for (int i = subs.Count - 1; i >= 0; i--)
            if (subs[i].Handler == handler)
                subs.RemoveAt(i);
        
        Local<T>.Dirty = true;
    }

    public static void RaiseLocal<T>(Entity target, ref T ev)
    {
        if (Local<T>.Dirty)
            RebuildLocalCache<T>();

        var cache = Local<T>.Cache;
        for (int i = 0; i < cache.Length; i++)
            cache[i](target, ref ev);
    }

    private static void RebuildLocalCache<T>()
    {
        var subs = Local<T>.Subs;
        var cache = new LocalRefAction<T>[subs.Count];
        for (int i = 0; i < subs.Count; i++)
            cache[i] = subs[i].Handler;

        Local<T>.Cache = cache;
        Local<T>.Dirty = false;
    }
}