namespace SolTK;
using System;
using System.Collections.Generic;
using System.Linq;

public interface IProcessable
{
    public int Priority { get; }
    public void Process(double delta);
}

public static class Scheduler
{
    private static List<(Action<double>, int)> _systems = [];
    private static List<(Action<double>, int)> _simulationSystems = [];
    
    // Overrideable anywhere (Godot, Unity, Console)
    public static Action<string, Exception> OnError = (systemName, ex) =>
        Console.WriteLine($"Error in system {systemName}: {ex.Message}");
    

    public static void Register(IProcessable system) =>
        Register(system.Process, system.Priority);
    
    public static void RegisterSimulation(IProcessable system) =>
        RegisterSimulation(system.Process, system.Priority);
    

    public static void RegisterSimulation(Action<double> system, int priority)
    {
        _simulationSystems.Add((system, priority));
        _simulationSystems = [.. _simulationSystems.OrderBy(data => data.Item2)];
    }
    
    public static void Register(Action<double> system, int priority)
    {
        _systems.Add((system, priority));
        _systems = [.. _systems.OrderBy(data => data.Item2)];
    }
    
     public static void ProcessAll(double delta) =>
        ProcessList(_systems, delta);
    

    public static void ProcessAllSimulation(double delta) =>
        ProcessList(_simulationSystems, delta);
    
    
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