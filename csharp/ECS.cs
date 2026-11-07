namespace SolTK;

using SolTK.Components;
using fennecs;



public static class ECS
{
    static ECS()
    {
        Scheduler.Register(FlushDespawn, STKPriority.Flush);
    }

    private static readonly Lazy<World> _world = new(
        () => new World()
    );

    public static World World => _world.Value;
    public static Entity Entity(string? name = null)
    {
        var entity = World.Spawn();
        if (name != null)
            entity.Add(new Name(name));
        
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

    private static Stream<MarkDespawn> _markedDespawn = World.Stream<MarkDespawn>();
    private static void FlushDespawn(double _) => _markedDespawn.Despawn();
}