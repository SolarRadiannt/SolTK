namespace SolTK.CommonUtil;

using System;
using System.Collections.Generic;
using System.Diagnostics;

/// <summary>
/// A utility for tracking timestamps and managing cooldowns/intervals.
/// Equivalent to a high-precision os.clock() tracker.
/// </summary>
public class TimeStamper : IDisposable
{
    private static TimeSpan Now => TimeUtil.SimNow;

    private readonly Dictionary<string, TimeSpan> _timestamps = [];

    /// <summary>
    /// Gets the timestamp for the given ID. If it doesn't exist, it creates one for "now".
    /// </summary>
    public TimeSpan GetTimeStamp(string id)
    {
        if (!_timestamps.TryGetValue(id, out var stamp))
        {
            stamp = Now;
            _timestamps[id] = stamp;
        }
        return stamp;
    }

    /// <summary>
    /// Updates the timestamp for the given ID to "now" (or a specific time).
    /// </summary>
    public TimeStamper Update(string id, TimeSpan? now = null)
    {
        _timestamps[id] = now ?? Now;
        return this;
    }

    /// <summary>
    /// Updates multiple timestamps at once.
    /// </summary>
    public TimeStamper UpdateBatch(IEnumerable<string> ids, TimeSpan? now = null)
    {
        var currentNow = now ?? Now;
        foreach (var id in ids)
            _timestamps[id] = currentNow;
        
        return this;
    }

    /// <summary>
    /// Checks if the duration has passed. If the ID doesn't exist, it creates it and returns trueOnFirst.
    /// </summary>
    public bool Compare(string id, TimeSpan duration, bool trueOnFirst = false)
    {
        var now = Now;
        if (!_timestamps.TryGetValue(id, out var stamp))
        {
            _timestamps[id] = now;
            return trueOnFirst;
        }

        return (now - stamp) >= duration;
    }

    /// <summary>
    /// Checks if the duration has passed. If it has, it automatically updates the timestamp to "now".
    /// </summary>
    public bool CompareAndUpdate(string id, TimeSpan duration, bool trueOnFirst = false)
    {
        var now = Now;
        if (!_timestamps.TryGetValue(id, out var stamp))
        {
            _timestamps[id] = now;
            return trueOnFirst;
        }

        bool result = (now - stamp) >= duration;
        if (result)
            _timestamps[id] = now;
        
        return result;
    }

    public bool Clear(string id) => _timestamps.Remove(id);
    public void ClearAll() => _timestamps.Clear();

    public void Dispose()
    {
        ClearAll();
        GC.SuppressFinalize(this);
    }
}