namespace SolTK.CommonUtil;

using System;
using System.Diagnostics;

public static class TimeUtil
{
    // The high-precision, monotonic clock
    private static readonly Stopwatch _clock = Stopwatch.StartNew();
    private static TimeSpan _lastRealTime = TimeSpan.Zero;

    // Real-world time (keeps ticking even if the game is paused)
    public static TimeSpan RealNow => _clock.Elapsed;
    public static double RealNowD => RealNow.TotalSeconds;
    public static float RealNowF => (float)RealNow.TotalSeconds;
    
    // Simulation time (stops ticking when paused, affected by time scale)
    public static TimeSpan SimNow { get; private set; }
    public static double SimNowD => SimNow.TotalSeconds;
    public static float SimNowF => (float)SimNow.TotalSeconds;

    public static double TimeScale { get; set; } = 1.0;
    public static bool IsPaused => TimeScale <= 0.0;

    /// <summary>
    /// Advances the simulation time and returns the scaled delta.
    /// Call this EXACTLY ONCE per frame in your main loop.
    /// </summary>
    public static TimeSpan Tick()
    {
        var currentReal = _clock.Elapsed;
        var delta = currentReal - _lastRealTime;
        _lastRealTime = currentReal;

        if (TimeScale > 0)
        {
            var scaledDelta = TimeSpan.FromTicks((long)(delta.Ticks * TimeScale));
            SimNow += scaledDelta;
            return scaledDelta;
        }
        
        return TimeSpan.Zero;
    }
}