using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class SpriteSheetAnimationData : ScriptableObject
{
    public string animationName;
    public Sprite[] sprites;
    public int startIndex;
    public bool playOnStart = true;
    public float duration;
    public float frameDuration
    {
        get
        {
            DebugExtensions.LogVar(new { duration, sprites.Length , fd = duration / sprites.Length});
            
            return duration / sprites.Length;
        }
    }

    public SpriteSheetAnimationComponent.RepetitionType repetition = SpriteSheetAnimationComponent.RepetitionType.Loop;
}