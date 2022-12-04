using Unity.Entities;
using Unity.Burst;
using Unity.Jobs;
using UnityEngine;

public partial class SpriteSheetAnimationSystem : SystemBase
{
    public static void SetAnimation(Entity e, Entity animationRenderGroup, bool keepProgress = false) =>
        SetAnimationInternal(e, animationRenderGroup, World.DefaultGameObjectInjectionWorld.EntityManager, keepProgress);
    
    protected override void OnUpdate()
    {
        float elapsedTime = (float) UnityEngine.Time.realtimeSinceStartup;
        
        var entityToAnimationDefCmpRo = GetComponentLookup<SpriteSheetAnimationDefinitionComponent>(true);
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
    
    private static void SetAnimationInternal(Entity entity, Entity animationRenderGroup, EntityManager eManager, bool keepProgress = false)
    {
        RenderGroup.AddToNewRenderGroup(entity, animationRenderGroup);

        var animDefCmp = eManager.GetComponentData<SpriteSheetAnimationDefinitionComponent>(animationRenderGroup);

        var animCmp = eManager.GetComponentData<SpriteSheetAnimationComponent>(entity);
        if (keepProgress)
        {
            var currentAnimDefCmp = eManager.GetComponentData<SpriteSheetAnimationDefinitionComponent>(animCmp.CurrentAnimation);
            Debug.Assert(currentAnimDefCmp.SpriteCount == animDefCmp.SpriteCount);
            Debug.Assert(Mathf.Approximately(currentAnimDefCmp.Duration, animDefCmp.Duration));
        }
        else
        {
            eManager.SetComponentData(entity, new SpriteIndex {Value = 0});
            animCmp.FrameStartTime = UnityEngine.Time.realtimeSinceStartup;
        }

        animCmp.Status = ESpriteSheetAnimationStatus.Playing;
        animCmp.CurrentAnimation = animationRenderGroup;

        eManager.SetComponentData(entity, animCmp);

        // DebugExtensions.LogVar(new { e = entity.Stringify(), anim = animationRenderGroup.Stringify() }, "Anim change", addCurrFrame:true);
    }
}