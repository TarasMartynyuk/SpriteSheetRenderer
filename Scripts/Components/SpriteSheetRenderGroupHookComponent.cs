using Unity.Entities;

public struct SpriteSheetRenderGroupHookComponent : IComponentData
{
    public int IndexInRenderGroup;
    public Entity SpritesheetRenderGroup;
}