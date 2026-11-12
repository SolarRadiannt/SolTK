namespace SolTK;

using fennecs;

public static class ECS
{
    private static readonly Stream<MarkDespawn> _markedDespawn = World.Stream<MarkDespawn>();
    static ECS()
    {
        Scheduler.Register(_ => _markedDespawn.Despawn(), STKPriority.Flush);
    }

    private static readonly Lazy<World> _world = new(
        () => new World()
    );

    public static World World => _world.Value;
    public static Entity Entity(string? name = null)
    {
        var entity = World.Spawn();
        if (name != null)
            entity.Add<Name>(name);
        
        return entity;
    }

    public static Entity QueueDespawn(Entity entity) => entity.Add<MarkDespawn>();
    
    public static string Name(Entity entity)
    {
        if (entity.Has<Name>())
            return entity.Ref<Name>().Value;
        else
            return entity.ToRaw().ToString();
    }
}