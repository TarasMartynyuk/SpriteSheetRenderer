using System;

[Serializable]
public struct SerializableNullable<T>
    where T : unmanaged
{
    public T Value;
    public bool HasValue;
}