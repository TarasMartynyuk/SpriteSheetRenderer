using Unity.Entities;

public struct SpriteSheetAnimationComponent : IComponentData
{
    public Entity CurrentAnimation;
    public float FrameStartTime;
    public bool IsPlaying;
    
    
    // public RepetitionType repetition;

    // public float frameDuration;
    // public int maxSprites;
}