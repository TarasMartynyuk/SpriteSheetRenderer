using Unity.Entities;
using Unity.Mathematics;

public struct SpriteIndex : IComponentData
{
    public int Value;
}

public struct SpriteSheetColor : IComponentData
{
    public float4 Value;
}

public struct SpriteSheetRenderGroupHookComponent : IComponentData
{
    public int IndexInRenderGroup;
    public Entity SpritesheetRenderGroup;

    public override string ToString() => $"I: {IndexInRenderGroup}, Group: {SpritesheetRenderGroup.Stringify()}";
}

public struct SpriteSheetAnimationComponent : IComponentData
{
    public Entity CurrentAnimation;
    public float FrameStartTime;

    public ESpriteSheetAnimationStatus Status;

    // true for the first frame when the animation event sprite(keyframe) is rendered
    public bool IsAnimationEventTriggeredThisFrame;
}

public enum ESpriteSheetAnimationStatus
{
    Invalid,
    Playing,
    Paused,
    Ended
}