using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[System.Serializable]
public abstract class SpriteSheetAnimator : ScriptableObject
{
    public SpriteSheetAnimationData[] animations;
    public int defaultAnimationIndex;

    public float speed = 1;

    public int GetAnimationIndex(string animationName)
    {
        for (var i = 0; i < animations.Length; i++)
        {
            if (animations[i].animationName == animationName)
                return i;
        }
        return -1;
    }
}