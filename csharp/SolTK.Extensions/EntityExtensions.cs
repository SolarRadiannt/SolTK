namespace SolTK.Extensions;
using fennecs;
using System.Linq;
using System.Diagnostics.CodeAnalysis;

public static class EntityExtensions
{
    private static World world => ECS.World;
	public static string Name(this Entity entity) => ECS.Name(entity);
	public static Entity QueueDespawn(this Entity entity) => ECS.QueueDespawn(entity);

	public static Entity[] Targets<R>(this Entity entity) =>
        [.. world.Query()
			.Has<R>(entity)
			.Compile()];
	
	public static Entity TargetFirst<R>(this Entity entity) =>
		world.Query()
			.Has<R>(entity)
			.Compile()
			.First();
	
	public static bool TryTargetFirst<R>(this Entity entity, out Entity target) =>
		world.Query()
			.Has<R>(entity)
			.Compile()
			.TryFirst(out target);
	
	public static bool TryRead<T>(this Entity entity, Match match, [MaybeNullWhen(false)] out T component)
		where T : notnull
	{
		if (entity.Has<T>(match))
		{
			component = entity.Ref<T>(match);
			return true;
		}
		
		component = default;
		return false;
	}
	
	public static bool TryRead<T>(this Entity entity, Entity relation, [MaybeNullWhen(false)] out T component)
        where T : notnull
    {
		if (entity.Has<T>(relation))
		{
			component = entity.Ref<T>(relation);
			return true;
		}
		
		component = default;
		return false;
	}
	
	/// <summary>
    /// Tries to read a copy of the plain component of type <typeparamref name="T"/>.
    /// </summary>
	public static bool TryRead<T>(this Entity entity, [MaybeNullWhen(false)] out T component)
        where T : notnull
    {
		if (entity.Has<T>())
		{
			component = entity.Ref<T>();
			return true;
		}
		component = default;
		return false;
	}

	public static bool TryAdd<T>(this Entity entity, T component)
        where T : notnull
    {
		if (entity.Has<T>()) return false;
		entity.Add(component);
		return true;
	}
	public static bool TryAdd<T>(this Entity entity)
		where T : struct
	{
		if (entity.Has<T>()) return false;
		entity.Add<T>();

		return true;
	}
	public static bool TryAdd<R>(this Entity entity, R value, Entity target) where R : notnull
	{
		if (entity.Has<R>(target)) return false;
		entity.Add(value, target);
		return true;
	}

	public static void Set<T>(this Entity entity, T component)
        where T : notnull
    {
		if (entity.Has<T>())
			entity.Ref<T>() = component;
		else
			entity.Add(component);
	}

	// ============ UNIVERSAL (Structs & Classes) ============

	public static bool TryRemove<T>(this Entity entity) // ✅ No constraint!
        where T : notnull
    {
		if (!entity.Has<T>()) return false;
		entity.Remove<T>();
		return true;
	}

	public static bool TryRemove<R>(this Entity entity, Entity target) where R : notnull
    {
		if (!entity.Has<R>(target)) return false;
		entity.Remove<R>(target);
		return true;
	}

	// ============ Multi-Checks ============
	public static bool HasAll<T1, T2>(this Entity entity) where T1 : notnull where T2 : notnull
		=> entity.Has<T1>() && entity.Has<T2>();
	public static bool HasAll<T1, T2, T3>(this Entity entity) where T1 : notnull where T2 : notnull where T3 : notnull
		=> entity.Has<T1>() && entity.Has<T2>() && entity.Has<T3>();
	public static bool HasAll<T1, T2, T3, T4>(this Entity entity) where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull
		=> entity.Has<T1>() && entity.Has<T2>() && entity.Has<T3>() && entity.Has<T4>();
	public static bool HasAll<T1, T2, T3, T4, T5>(this Entity entity) where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull
		=> entity.Has<T1>() && entity.Has<T2>() && entity.Has<T3>() && entity.Has<T4>() && entity.Has<T5>();
	

	public static bool HasAny<T1, T2>(this Entity entity) where T1 : notnull where T2 : notnull
		=> entity.Has<T1>() || entity.Has<T2>();
	public static bool HasAny<T1, T2, T3>(this Entity entity) where T1 : notnull where T2 : notnull where T3 : notnull
		=> entity.Has<T1>() || entity.Has<T2>() || entity.Has<T3>();
	public static bool HasAny<T1, T2, T3, T4>(this Entity entity) where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull
		=> entity.Has<T1>() || entity.Has<T2>() || entity.Has<T3>() || entity.Has<T4>();
	public static bool HasAny<T1, T2, T3, T4, T5>(this Entity entity) where T1 : notnull where T2 : notnull where T3 : notnull where T4 : notnull where T5 : notnull
		=> entity.Has<T1>() || entity.Has<T2>() || entity.Has<T3>() || entity.Has<T4>() || entity.Has<T5>();
}