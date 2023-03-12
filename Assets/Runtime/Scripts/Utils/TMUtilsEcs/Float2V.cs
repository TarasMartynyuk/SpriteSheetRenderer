using Unity.Mathematics;

namespace SmokGnu.SpriteSheetRenderer.Utils.TMUtilsEcs
{
    public static class Float2V
    {
        public static float2 One => new(1, 1);
        public static float2 BasisX => new(1, 0);
        public static float2 BasisY => new(0, 1);
    }
}
