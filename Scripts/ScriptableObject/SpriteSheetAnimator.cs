using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[CreateAssetMenu(fileName = "SpriteSheetAnimator", menuName = "SpriteSheetRenderer/SpriteSheetAnimator", order = 0)]
public class SpriteSheetAnimator : ScriptableObject
{
    public SpriteSheetAnimationScriptable[] animations;
    public int defaultAnimationIndex;
    public float Radius;

    public SpriteSheetAnimationScriptable GetAnimation(Entity renderGroup) => 
        Array.Find(animations, a => a.RenderGroup == renderGroup);

    public int GetAnimationIndex(string animationName)
    {
        for (var i = 0; i < animations.Length; i++)
        {
            if (animations[i].AnimationName == animationName)
                return i;
        }

        return -1;
    }
}