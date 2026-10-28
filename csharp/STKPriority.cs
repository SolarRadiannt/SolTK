namespace SolTK;
public static class STKPriority
{
    // Generic priorities v
    public static readonly int Lowest   = -100_000;
    public static readonly int Lower    = -10_000;
    public static readonly int Low      = -1_000;
    public static readonly int High     = 1_000;
    public static readonly int Higher   = 10_000;
    public static readonly int Highest  = 100_000;
    
    // Higher level priorities
    public static readonly int Flush            = -10_000_000;
    public static readonly int Init             = 10_000_000;
    public static readonly int Action           = Higher;
    public static readonly int Interception     = High + 200;
    public static readonly int Transformation   = High + 100;
    public static readonly int Applying         = High;
    public static readonly int Default          = 0;
}