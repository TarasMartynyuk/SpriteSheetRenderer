using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public static class SpriteSheetManager
{
    public static List<RenderInformation> renderInformation = new();

    static EntityManager EntityManager => World.DefaultGameObjectInjectionWorld.EntityManager;

    // public static Entity Instantiate(EntityArchetype archetype, string spriteSheetName)
    // {
    //     Entity e = EntityManager.CreateEntity(archetype);
    //     Material material = SpriteSheetCache.Instance.GetMaterial(spriteSheetName);
    //     int bufferID = DynamicBufferManager.AddDynamicBuffers(DynamicBufferManager.GetEntityBuffer(material), material);
    //
    //     BufferHook bh = new BufferHook {bufferID = bufferID, bufferEnityID = DynamicBufferManager.GetEntityBufferID(material)};
    //     EntityManager.SetComponentData(e, bh);
    //     return e;
    // }

    public static Entity Instantiate(EntityArchetype archetype, SpriteSheetAnimator animator)
    {
        Entity e = EntityManager.CreateEntity(archetype);
        Init(e, animator);
        return e;
    }

    public static void Init(Entity spriteSheetEntity, SpriteSheetAnimator animator)
    {
        var startAnim = animator.animations[animator.defaultAnimationIndex];
        SetAnimationInternal(spriteSheetEntity, startAnim.RenderGroup);
    }

    public static void SetAnimation(Entity e, Entity animationRenderGroup, bool keepProgress = false)
    {
        SetAnimationInternal(e, animationRenderGroup, keepProgress);
    }

    // public static void SetAnimation(EntityCommandBuffer commandBuffer, Entity e, SpriteSheetAnimationScriptable animation, BufferHook hook)
    // {
    //     Material oldMaterial = DynamicBufferManager.GetMaterial(hook.bufferEnityID);
    //     string oldAnimation = SpriteSheetCache.Instance.GetMaterialName(oldMaterial);
    //     if (animation.animationName != oldAnimation)
    //     {
    //         Material material = SpriteSheetCache.Instance.GetMaterial(animation.animationName);
    //         //clean old buffer
    //         DynamicBufferManager.RemoveBuffer(oldMaterial, hook.bufferID);
    //
    //         //use new buffer
    //         int bufferID = DynamicBufferManager.AddDynamicBuffers(DynamicBufferManager.GetEntityBuffer(material), material);
    //         BufferHook bh = new BufferHook {bufferID = bufferID, bufferEnityID = DynamicBufferManager.GetEntityBufferID(material)};
    //
    //         commandBuffer.SetComponent(e, bh);
    //     }
    //
    //     commandBuffer.SetComponent(e,
    //         new SpriteSheetAnimationComponent
    //         {
    //             maxSprites = animation.sprites.Length, isPlaying = animation.playOnStart, frameDuration = animation.frameDuration,
    //             repetition = animation.repetition
    //         });
    //     commandBuffer.SetComponent(e, new SpriteIndex {Value = animation.startIndex});
    // }

    public static void DestroyEntity(Entity e)
    {
        RenderGroupManager.RemoveFromRenderGroup(EntityManager.GetComponentData<SpriteSheetRenderGroupHookComponent>(e));
        EntityManager.DestroyEntity(e);
    }

    public static void DestroyEntity(EntityCommandBuffer commandBuffer, Entity e, SpriteSheetRenderGroupHookComponent hook)
    {
        throw new NotImplementedException();
        // commandBuffer.DestroyEntity(e);
        // Material material = DynamicBufferManager.GetMaterial(hook.bufferEnityID);
        // DynamicBufferManager.RemoveBuffer(material, hook.bufferID);
    }

    // public static void RecordSpriteSheet(Sprite[] sprites, string spriteSheetName)
    // {
    //     KeyValuePair<Material, float4[]> atlasData = SpriteSheetCache.Instance.BakeSprites(sprites, spriteSheetName);
    //     RenderGroupManager.CreateRenderGroup(atlasData.Key);
    //     RenderGroupManager.BakeUvBuffer(atlasData.Key, atlasData);
    //     renderInformation.Add(new RenderInformation(atlasData.Key, RenderGroupManager.GerRenderGroup(atlasData.Key)));
    // }

    public static void RecordAnimator(SpriteSheetAnimator animator)
    {
        for (var i = 0; i < animator.animations.Length; i++)
        {
            var animation = animator.animations[i];
            var atlasData = SpriteSheetCache.Instance.BakeSprites(animation.Sprites, animation.AnimationName);
            var newRenderGroup = RenderGroupManager.CreateRenderGroup(atlasData.Key, atlasData.Value);
            animation.Init(i, newRenderGroup);

            renderInformation.Add(new RenderInformation(atlasData.Key, newRenderGroup));
        }
    }

    public static void CleanBuffers()
    {
        for (int i = 0; i < renderInformation.Count; i++)
            renderInformation[i].DestroyBuffers();
        renderInformation.Clear();
    }

    public static void ReleaseUvBuffer(int bufferID)
    {
        if (renderInformation[bufferID].uvBuffer != null)
            renderInformation[bufferID].uvBuffer.Release();
    }

    public static void ReleaseBuffer(int bufferID)
    {
        if (renderInformation[bufferID].matrixBuffer != null)
            renderInformation[bufferID].matrixBuffer.Release();
        if (renderInformation[bufferID].colorsBuffer != null)
            renderInformation[bufferID].colorsBuffer.Release();
        //if(renderInformation[bufferID].uvBuffer != null)
        //renderInformation[bufferID].uvBuffer.Release();
        if (renderInformation[bufferID].indexBuffer != null)
            renderInformation[bufferID].indexBuffer.Release();
    }

    private static void SetAnimationInternal(Entity e, Entity animationRenderGroup,
        bool keepProgress = false)
    {
        var groupHookCmp = EntityManager.GetComponentData<SpriteSheetRenderGroupHookComponent>(e);

        if (groupHookCmp.SpritesheetRenderGroup != Entity.Null)
            RenderGroupManager.RemoveFromRenderGroup(groupHookCmp.SpritesheetRenderGroup, groupHookCmp.IndexInRenderGroup);

        //use new buffer
        int indexInGroup = RenderGroupManager.AddToGroup(animationRenderGroup);
        groupHookCmp = new SpriteSheetRenderGroupHookComponent
            {IndexInRenderGroup = indexInGroup, SpritesheetRenderGroup = animationRenderGroup};

        EntityManager.SetComponentData(e, groupHookCmp);

        var animDefCmp = EntityManager.GetComponentData<SpriteSheetAnimationDefinitionComponent>(animationRenderGroup);

        var animCmp = EntityManager.GetComponentData<SpriteSheetAnimationComponent>(e);
        if (keepProgress)
        {
            var currentAnimDefCmp = EntityManager.GetComponentData<SpriteSheetAnimationDefinitionComponent>(animCmp.CurrentAnimation);
            Debug.Assert(currentAnimDefCmp.SpriteCount == animDefCmp.SpriteCount);
            Debug.Assert(Mathf.Approximately(currentAnimDefCmp.Duration, animDefCmp.Duration));
        }
        else
        {
            EntityManager.SetComponentData(e, new SpriteIndex {Value = 0});
            animCmp.FrameStartTime = Time.realtimeSinceStartup;
        }

        animCmp.Status = ESpriteSheetAnimationStatus.Playing;
        animCmp.CurrentAnimation = animationRenderGroup;

        EntityManager.SetComponentData(e, animCmp);
    }
}