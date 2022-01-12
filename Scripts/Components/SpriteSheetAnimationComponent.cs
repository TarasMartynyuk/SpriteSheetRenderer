using Unity.Entities;

public enum ESpriteSheetAnimationStatus
{
    Invalid,
    Playing,
    Paused,
    Ended
}

public struct SpriteSheetAnimationComponent : IComponentData
{
    public Entity CurrentAnimation;
    public float FrameStartTime;
    public ESpriteSheetAnimationStatus Status;
    // true for the first frame during witch the animation sprite(keyframe) is rendered
    public bool IsAnimationEventTriggeredThisFrame;
}