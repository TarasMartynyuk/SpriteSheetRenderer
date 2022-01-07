using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[CreateAssetMenu(fileName = "SpriteSheetAnimator", menuName = "SpriteSheetRenderer/SpriteSheetAnimator", order = 0)]
[System.Serializable]
public class SpriteSheetAnimator : ScriptableObject
{
    public SpriteSheetAnimationData[] animations;
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