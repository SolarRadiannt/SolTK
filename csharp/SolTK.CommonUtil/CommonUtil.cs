namespace SolTK.CommonUtil;


public static class CommonUtil
{
    public static Dictionary<T, bool> ToLookup<T>(List<T> list)
    where T : notnull
    {
        Dictionary<T, bool> lookup = [];

        foreach (var item in list)
            lookup[item] = true;
        
        return lookup;
    }

}