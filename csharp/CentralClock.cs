using System.Diagnostics;

namespace SolTK;

public static class CentralClock
{
    private static readonly Stopwatch _clock = Stopwatch.StartNew();
    private static TimeSpan _lastRealTime = TimeSpan.Zero;

    // Real-world time (ignores pause/time scale)
    public static TimeSpan RealNow => _clock.Elapsed;
    public static double RealNowD => RealNow.TotalSeconds;
    public static float RealNowF => (float)RealNow.TotalSeconds;
    
    // Simulation time (freezes on pause, scales with TimeScale)
    public static TimeSpan SimNow { get; private set; }
    public static double SimNowD => SimNow.TotalSeconds;
    public static float SimNowF => (float)SimNow.TotalSeconds;

    public static double TimeScale { get; set; } = 1.0;
    public static bool IsPaused => TimeScale <= 0.0;

    /// <summary>
    /// Advances the real clock. Call exactly ONCE per main loop frame.
    /// Returns the scaled variable delta (for UI/Input/Rendering).
    /// </summary>
    public static TimeSpan TickReal()
    {
        var currentReal = _clock.Elapsed;
        var delta = currentReal - _lastRealTime;
        _lastRealTime = currentReal;

        if (TimeScale > 0)
            return TimeSpan.FromTicks((long)(delta.Ticks * TimeScale));
        
        return TimeSpan.Zero;
    }

    /// <summary>
    /// Advances the simulation clock. Call inside a fixed-timestep loop.
    /// </summary>
    public static void TickSimulation(TimeSpan fixedDelta)
    {
        if (TimeScale > 0)
        {
            var scaledDelta = TimeSpan.FromTicks((long)(fixedDelta.Ticks * TimeScale));
            SimNow += scaledDelta;
        }
    }
}