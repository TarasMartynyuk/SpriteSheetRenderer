using Unity.Entities;

public struct AnimationChangeCommandBufferElement : IBufferElementData
{
    public Entity Target;
    public Entity RenderGroupToSet;
    public bool KeepProgress;
}
