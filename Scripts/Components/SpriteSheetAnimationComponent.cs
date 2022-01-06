using Unity.Entities;

public struct SpriteSheetAnimationComponent : IComponentData
{
    public enum RepetitionType
    {
        Once,
        Loop,
        PingPong
    }

    public RepetitionType repetition;

    // public int elapsedFrames;
    public float frameDuration;
    public float frameStartTime;
    
    //how many frames does this animation takes to move to the next sprite
    // public int samples;
    public bool isPlaying;
    public int maxSprites;
}