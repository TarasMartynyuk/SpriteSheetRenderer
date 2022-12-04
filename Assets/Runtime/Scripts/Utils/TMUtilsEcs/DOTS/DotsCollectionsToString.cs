using System.Text;
using Unity.Collections;
using Unity.Entities;

public static class DotsCollectionsToString
{
    public static string Stringify(this Entity e)
    {
        return World.DefaultGameObjectInjectionWorld.EntityManager.GetName(e);
    }
    
    public static string Stringify<T>(this NativeArray<T> array)
        where T : unmanaged
    {
        var sb = new StringBuilder('[');
        foreach (var item in array)
        {   
            sb.Append(item);
            sb.Append(", ");
        }
        sb.Append(']');
        return sb.ToString();
    }

    public static string Stringify<T>(this DynamicBuffer<T> buff)
        where T : unmanaged
        => Stringify(buff.AsNativeArray());

    public static string Stringify<T>(this NativeList<T> list)
        where T : unmanaged
        => Stringify(list.AsArray());
}
