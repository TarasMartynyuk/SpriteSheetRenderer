using Unity.Mathematics;

namespace SmokGnu.SpriteSheetRenderer.Utils.TMUtilsEcs
{
    public static class Float3V
    {
        public static float3 One => new float3(1, 1, 1);

        public static float3 BasisX => new float3(1, 0, 0);
        public static float3 BasisY => new float3(0, 1, 0);
        public static float3 BasisZ => new float3(0, 0, 1);
    }
}
