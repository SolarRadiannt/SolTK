namespace SolTK;
using System;
using System.Collections.Generic;
using System.Linq;

public interface IProcessable
{
    public int Priority { get; }
    public void Process(double delta);
}

public static class SPriority
{
    // Generic priorities v
    public static readonly int Lowest  = -100_000;
    public static readonly int Lower   = -10_000;
    public static readonly int Low     = -1_000;
    public static readonly int High    = 1_000;
    public static readonly int Higher  = 10_000;
    public static readonly int Highest = 100_000;
    
    // Higher level priorities
    public static readonly int Flush          = -10_000_000;
    public static readonly int Init           = 10_000_000;
    public static readonly int Action         = Higher;
    public static readonly int Interception   = High + 200;
    public static readonly int Transformation = High + 100;
    public static readonly int Applying       = High;
    public static readonly int Default        = 0;
}

public static class Scheduler
{
    private static List<(Action<double>, int)> _systems = [];
    private static List<(Action<double>, int)> _physicsSystems = [];
    
    // The user can override this anywhere (Godot, Unity, Console)
    public static Action<string, Exception> OnError = (systemName, ex) =>
        Console.WriteLine($"Error in system {systemName}: {ex.Message}");
    
    public static void Register(IProcessable system)
    {
        Register(system.Process, system.Priority);
    }
    
    public static void RegisterPhysics(IProcessable system)
    {
        RegisterPhysics(system.Process, system.Priority);
    }
    
    public static void RegisterPhysics(Action<double> system, int priority)
    {
        _physicsSystems.Add((system, priority));
        _physicsSystems = [.. _physicsSystems.OrderBy(data => data.Item2)];
    }
    
    public static void Register(Action<double> system, int priority)
    {
        _systems.Add((system, priority));
        _systems = [.. _systems.OrderBy(data => data.Item2)];
    }
    
     public static void ProcessAll(double delta) =>
        ProcessList(_systems, delta);
    

    public static void ProcessAllPhysics(double delta) =>
        ProcessList(_physicsSystems, delta);
    
    
    private static void ProcessList(List<(Action<double>, int)> systems, double delta)
    {
        // Iterate backwards: Zero allocations, safe removal
        for (int i = systems.Count - 1; i >= 0; i--)
        {
            var system = systems[i].Item1;
            
            try
            {
                system(delta);
            }
            catch (Exception ex)
            {
                OnError(system.Method.DeclaringType?.Name ?? system.Method.Name, ex);
                systems.RemoveAt(i); 
            }
        }
    }
}