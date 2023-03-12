using Unity.Mathematics;

namespace SmokGnu.SpriteSheetRenderer.Utils.TMUtilsEcs
{
    public static class Float3V
    {
        public static float3 One => new(1, 1, 1);

        public static float3 BasisX => new(1, 0, 0);
        public static float3 BasisY => new(0, 1, 0);
        public static float3 BasisZ => new(0, 0, 1);
    }
}
