namespace SolTK;
using fennecs;
using SolTK.Components;
public struct Transient;

public class ETransient : IService, IProcessable
{
	private static World _world = ECS.World;
    private static readonly Stream<Transient> _transients = _world.Stream<Transient>();

    public int Priority => SPriority.Flush;
    public void Init() => Scheduler.Register(this);

	public static Entity Spawn(string? name = null) =>
		ECS.Entity(name)
			.Add<Transient>();
    
    public static string Name(Entity ent) => ent.Ref<Name>().Value;

    public void Process(double _) => _transients.Despawn();
}