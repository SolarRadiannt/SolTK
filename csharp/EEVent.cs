namespace SolTK;
using SolTK.Components;

public struct EventEntity;
public struct EventCancelled;

public static class EEVent
{
    public static Entity Spawn(string? name = null) =>
		ETransient.Spawn(name)
			.Add<EventEntity>();
    
    public static readonly EntityTemplate<Name> Template = ECS.World.Template()
        .Needs<Name>()
        .Add<Transient>()
        .Add<EventEntity>();
}