namespace SolTK;

public struct MarkDespawn;
public struct Name(string name)
{
    public string Value = name;
    public static implicit operator Name(string name) => new(name);
};