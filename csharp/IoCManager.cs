namespace SolTK;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public interface ISystem
{
    public void Init();
}

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class DependencyAttribute : Attribute { }

public static class IoCManager
{
    private static readonly Dictionary<Type, object> _services = [];

    public static void InitAll(Assembly? targetAssembly)
    {
        targetAssembly ??= Assembly.GetCallingAssembly(); 

        // 2. Look for the interface instead of the base class
        var systemTypes = targetAssembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(ISystem).IsAssignableFrom(t))
            .ToList();

        foreach (var type in systemTypes)
        {
            if (!_services.ContainsKey(type))
            {
                _services[type] = Activator.CreateInstance(type)!;
            }
        }

        foreach (var instance in _services.Values)
        {
            InjectDependencies(instance);
        }

        foreach (var instance in _services.Values)
        {
            // 3. Cast to the interface
            if (instance is ISystem system)
            {
                system.Init();
            }
        }
    }

    private static void InjectDependencies(object target)
    {
        var fields = target.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        foreach (var field in fields)
        {
            if (field.GetCustomAttribute<DependencyAttribute>() != null)
            {
                var depType = field.FieldType;
                if (_services.TryGetValue(depType, out var instance))
                {
                    field.SetValue(target, instance);
                }
                else
                {
                    throw new InvalidOperationException($"Dependency {depType.Name} required by {target.GetType().Name} was not found!");
                }
            }
        }
    }
}