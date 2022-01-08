using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class SpriteSheetAnimationData : ScriptableObject
{
    public Sprite[] sprites;
    public Sprite SpriteSheet;
    
    public int startIndex;
    public bool playOnStart = true;
    public float duration;
    public float frameDuration => duration / sprites.Length;

    public string fullName { get; private set;  }
    public string Name => animationName;
    [SerializeField] string animationName;

    public SpriteSheetAnimationComponent.RepetitionType repetition = SpriteSheetAnimationComponent.RepetitionType.Loop;

    public void Init(SpriteSheetAnimator parentAnimator)
    {
        fullName = $"{parentAnimator.animationsPrefix}_{animationName}";
    }
    
    public void RetrieveSheetSprites()
    {
        
    }
}