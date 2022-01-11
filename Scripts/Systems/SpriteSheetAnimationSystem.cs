using Unity.Entities;
using Unity.Burst;
using Unity.Jobs;
using UnityEngine;

public class SpriteSheetAnimationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float elapsedTime = (float) Time.ElapsedTime;

        var entityToAnimationDefCmpRo = GetComponentDataFromEntity<SpriteSheetAnimationDefinitionComponent>(true);
        Entities.ForEach((ref SpriteSheetAnimationComponent animCmp, ref SpriteIndex spriteIndexCmp)
                =>
            {
                if (!animCmp.IsPlaying)
                    return;

                var animationDefCmp = entityToAnimationDefCmpRo[animCmp.CurrentAnimation];

                float elapsedTimeThisFrame = elapsedTime - animCmp.FrameStartTime;
                if (elapsedTimeThisFrame < animationDefCmp.frameDuration)
                    return;

                switch (animationDefCmp.repetition)
                {
                    case RepetitionType.Once:
                        if (!NextWillReachEnd(animationDefCmp, spriteIndexCmp))
                        {
                            spriteIndexCmp.Value += 1;
                        }
                        else
                        {
                            animCmp.IsPlaying = true;
                        }

                        break;
                    case RepetitionType.Loop:
                        if (NextWillReachEnd(animationDefCmp, spriteIndexCmp))
                            spriteIndexCmp.Value = 0;
                        else
                            spriteIndexCmp.Value += 1;
                        break;
                }

                animCmp.FrameStartTime = elapsedTime;
            })
            .WithReadOnly(entityToAnimationDefCmpRo)
            .Schedule();
    }

    static bool NextWillReachEnd(in SpriteSheetAnimationDefinitionComponent animationDefCmp, SpriteIndex sprite)
    {
        return sprite.Value + 1 >= animationDefCmp.maxSprites;
    }
}