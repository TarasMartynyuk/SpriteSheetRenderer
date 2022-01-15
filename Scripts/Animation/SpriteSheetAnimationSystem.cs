using Unity.Entities;
using Unity.Burst;
using Unity.Jobs;
using UnityEngine;

public class SpriteSheetAnimationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float elapsedTime = (float) UnityEngine.Time.realtimeSinceStartup;
        
        var entityToAnimationDefCmpRo = GetComponentDataFromEntity<SpriteSheetAnimationDefinitionComponent>(true);
        Entities.ForEach((
                    // Entity e,
                    ref SpriteSheetAnimationComponent animCmp, ref SpriteIndex spriteIndexCmp) =>
            {
                animCmp.IsAnimationEventTriggeredThisFrame = false;

                if (animCmp.Status == ESpriteSheetAnimationStatus.Paused)
                    return;

                var animationDefCmp = entityToAnimationDefCmpRo[animCmp.CurrentAnimation];

                float elapsedTimeThisFrame = elapsedTime - animCmp.FrameStartTime;
                if (elapsedTimeThisFrame < animationDefCmp.FrameDuration)
                    return;

                switch (animationDefCmp.Repetition)
                {
                    case RepetitionType.Once:
                        if (!NextWillReachEnd(animationDefCmp, spriteIndexCmp))
                        {
                            spriteIndexCmp.Value += 1;
                        }
                        else
                        {
                            animCmp.Status = ESpriteSheetAnimationStatus.Ended;
                        }

                        break;
                    case RepetitionType.Loop:
                        if (NextWillReachEnd(animationDefCmp, spriteIndexCmp))
                            spriteIndexCmp.Value = 0;
                        else
                            spriteIndexCmp.Value += 1;
                        break;
                }

                animCmp.IsAnimationEventTriggeredThisFrame =
                    animationDefCmp.EventFrame.HasValue && spriteIndexCmp.Value == animationDefCmp.EventFrame;
                    
                // DebugExtensions.LogVar(new
                // {
                //     i = spriteIndexCmp.Value,
                //     animCmp.FrameStartTime
                // }, "frame advance " + $"{e.Stringify()} anim: {animCmp.CurrentAnimation.Stringify()} ", true);
                
                animCmp.FrameStartTime = elapsedTime;
            })
            .WithReadOnly(entityToAnimationDefCmpRo)
            // .WithoutBurst()
            // .Run();
        .Schedule();
    }

    static bool NextWillReachEnd(in SpriteSheetAnimationDefinitionComponent animationDefCmp, SpriteIndex sprite)
    {
        return sprite.Value + 1 >= animationDefCmp.SpriteCount;
    }
}