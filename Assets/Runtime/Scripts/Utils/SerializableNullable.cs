using System;

namespace SmokGnu.SpriteSheetRenderer.Utils
{
    [Serializable]
    public struct SerializableNullable<T>
        where T : unmanaged
    {
        public T Value;
        public bool HasValue;
    }
}