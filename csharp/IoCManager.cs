namespace SolTK;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public interface IService
{
    public void Init();
}

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class DependencyAttribute : Attribute { }

public static class IoCManager
{
    private static readonly Dictionary<Type, object> _services = [];
    public static Action<string, Exception> OnError = (systemName, ex) =>
        Console.WriteLine($"Error in IoC system {systemName}: {ex.Message}");
    
        public static void InitAll(Assembly? targetAssembly = null)
    {
        targetAssembly ??= Assembly.GetCallingAssembly();

        var systemTypes = targetAssembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(IService).IsAssignableFrom(t))
            .ToList();

        foreach (var type in systemTypes)
        {
            try
            {
                if (!_services.ContainsKey(type))
                    _services[type] = Activator.CreateInstance(type)!;
            }
            catch (Exception ex) { OnError(type.Name, ex); }
        }

        foreach (var instance in _services.Values)
        {
            try { InjectDependencies(instance); }
            catch (Exception ex) { OnError(instance.GetType().Name, ex); }
        }

        foreach (var instance in _services.Values)
        {
            if (instance is IService system)
            {
                try { system.Init(); }
                catch (Exception ex) { OnError(instance.GetType().Name, ex); }
            }
        }
    }

    public static void InjectDependencies(object target)
    {
        var fields = target.GetType().GetFields(
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        foreach (var field in fields)
        {
            if (field.GetCustomAttribute<DependencyAttribute>() == null) continue;

            var depType = field.FieldType;
            if (_services.TryGetValue(depType, out var instance))
                field.SetValue(target, instance);
            else
                throw new InvalidOperationException(
                    $"Dependency {depType.Name} required by {target.GetType().Name} was not found!");
        }
    }

        // 2. Add a Resolve method for manual retrieval
    public static T Resolve<T>() where T : class
    {
        if (_services.TryGetValue(typeof(T), out var instance))
            return (T)instance;
        
        throw new InvalidOperationException($"Service {typeof(T).Name} not found!");
    }

    // 3. Add a Register method for manual injection
    public static void Register<T>(T instance) where T : class
    {
        _services[typeof(T)] = instance;
    }
}