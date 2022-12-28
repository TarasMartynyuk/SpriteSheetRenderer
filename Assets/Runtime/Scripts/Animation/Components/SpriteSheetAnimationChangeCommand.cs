using Unity.Entities;

namespace SmokGnu.SpriteSheetRenderer.Animation.Components
{
    public struct AnimationChangeCommandBufferElement : IBufferElementData
    {
        public Entity Target;
        public Entity RenderGroupToSet;
        public bool KeepProgress;
    }
}
