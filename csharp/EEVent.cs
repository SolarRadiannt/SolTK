namespace SolTK;

public struct EventEntity;
public class EEVent : ISystem
{
    public static Entity Spawn(string? name = null) =>
		ETransient.Spawn(name)
			.Add<EventEntity>();
    public void Init() { }
}