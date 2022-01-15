using Unity.Entities;
using Unity.Mathematics;

public struct SpriteIndex : IComponentData
{
    public int Value;
}

public struct SpriteSheetColor : IComponentData
{
    public float4 color;
}

public struct SpriteSheetRenderGroupHookComponent : IComponentData
{
    public int IndexInRenderGroup;
    public Entity SpritesheetRenderGroup;
}

public struct SpriteSheetAnimationComponent : IComponentData
{
    public Entity CurrentAnimation;
    public float FrameStartTime;

    public ESpriteSheetAnimationStatus Status;

    // true for the first frame when the animation sprite(keyframe) is rendered
    public bool IsAnimationEventTriggeredThisFrame;
}

public enum ESpriteSheetAnimationStatus
{
    Invalid,
    Playing,
    Paused,
    Ended
}