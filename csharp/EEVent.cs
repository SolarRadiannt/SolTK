namespace SolTK;

public struct EventEntity;
public class EEVent : ISystem
{
    public static Entity Spawn(string? name) =>
		ETransient.Spawn(name)
			.Add<EventEntity>();
    public void Init() { }
}