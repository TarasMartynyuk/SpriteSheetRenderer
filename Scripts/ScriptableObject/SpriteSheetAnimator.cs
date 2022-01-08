using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpriteSheetAnimator", menuName = "SpriteSheetRenderer/SpriteSheetAnimator", order = 0)]
public class SpriteSheetAnimator: ScriptableObject {
    
    public SpriteSheetAnimationDataScriptable[] animations;
    public int defaultAnimationIndex;

    public string animationsPrefix;

    // called manually - Awake does not work without domain reload on play
    public void Init()
    {
        foreach (var animation in animations)
        {
            animation.Init(this);
        }
    }

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