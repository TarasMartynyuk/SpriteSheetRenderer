using System;
using Unity.Entities;
using UnityEngine;

namespace SmokGnu.SpriteSheetRenderer.ScriptableObject
{
    [CreateAssetMenu(fileName = "SpriteSheetAnimator", menuName = "SpriteSheetRenderer/SpriteSheetAnimator", order = 0)]
    public class SpriteSheetAnimator : UnityEngine.ScriptableObject
    {
        public SpriteSheetAnimationScriptable[] animations;
        public int defaultAnimationIndex;

        public SpriteSheetAnimationScriptable GetAnimation(Entity renderGroup) => 
            Array.Find(animations, a => a.RenderGroup == renderGroup);

        public int GetAnimationIndex(string animationName)
        {
            for (var i = 0; i < animations.Length; i++)
            {
                if (animations[i].name == animationName)
                    return i;
            }

            return -1;
        }
    }
}