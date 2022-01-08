using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpriteSheetAnimator", menuName = "SpriteSheetRenderer/SpriteSheetAnimator", order = 0)]
public class SpriteSheetAnimator: ScriptableObject {
    
    public SpriteSheetAnimationData[] animations;
    public int defaultAnimationIndex;

    public int GetAnimationIndex(string animationName)
    {
        for (var i = 0; i < animations.Length; i++)
        {
            if (animations[i].Name == animationName)
                return i;
        }
        return -1;
    }
    
}