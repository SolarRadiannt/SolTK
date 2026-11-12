namespace SolTK;
using fennecs;
public struct Transient;

public class ETransient : IService
{
	private static World _world = ECS.World;
    private static readonly Stream<Transient> _transients = _world.Stream<Transient>();
    public void Init() => Scheduler.Register(_ => _transients.Despawn(), STKPriority.Flush);

	public static Entity Spawn(string? name = null) =>
		ECS.Entity(name)
			.Add<Transient>();

    public static readonly EntityTemplate<Name> Template = _world.Template()
        .Add<Transient>()
        .Needs<Name>();
}