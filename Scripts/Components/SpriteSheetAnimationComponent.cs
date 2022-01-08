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

    public float frameDuration;
    public float frameStartTime;

    // in animator list
    public int animationIndex;
    public bool isPlaying;
    public int maxSprites;
}