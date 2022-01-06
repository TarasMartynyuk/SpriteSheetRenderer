using Unity.Entities;
using Unity.Burst;
using Unity.Jobs;
using UnityEngine;

public class SpriteSheetAnimationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float elapsedTime = (float) Time.ElapsedTime;
        
        Entities.ForEach((ref SpriteSheetAnimationComponent animCmp, ref SpriteIndex spriteIndexCmp)
                =>
            {
                if (!animCmp.isPlaying)
                    return;

                float elapsedTimeThisFrame = elapsedTime - animCmp.frameStartTime;
                if (elapsedTimeThisFrame < animCmp.frameDuration)
                    return;

                switch (animCmp.repetition)
                {
                    case SpriteSheetAnimationComponent.RepetitionType.Once:
                        if (!NextWillReachEnd(animCmp, spriteIndexCmp))
                        {
                            spriteIndexCmp.Value += 1;
                        }
                        else
                        {
                            animCmp.isPlaying = true;
                        }

                        break;
                    case SpriteSheetAnimationComponent.RepetitionType.Loop:
                        if (NextWillReachEnd(animCmp, spriteIndexCmp))
                            spriteIndexCmp.Value = 0;
                        else
                            spriteIndexCmp.Value += 1;
                        break;
                }

                animCmp.frameStartTime = elapsedTime;
            })
            .Schedule();
    }

    static bool NextWillReachEnd(SpriteSheetAnimationComponent anim, SpriteIndex sprite)
    {
        return sprite.Value + 1 >= anim.maxSprites;
    }
}