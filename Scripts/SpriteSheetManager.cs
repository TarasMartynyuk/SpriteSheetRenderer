using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public static class SpriteSheetManager
{
    public static List<RenderInformation> renderInformation = new();

    public static EntityManager EntityManager => World.DefaultGameObjectInjectionWorld.EntityManager;

    public static Entity Instantiate(EntityArchetype archetype, string spriteSheetName)
    {
        Entity e = EntityManager.CreateEntity(archetype);
        Material material = SpriteSheetCache.Instance.GetMaterial(spriteSheetName);
        int bufferID = DynamicBufferManager.AddDynamicBuffers(DynamicBufferManager.GetEntityBuffer(material), material);

        BufferHook bh = new BufferHook {bufferID = bufferID, bufferEnityID = DynamicBufferManager.GetEntityBufferID(material)};
        EntityManager.SetComponentData(e, bh);
        return e;
    }

    public static Entity Instantiate(EntityArchetype archetype, SpriteSheetAnimator animator)
    {
        Entity e = EntityManager.CreateEntity(archetype);
        Init(e, animator);
        return e;
    }

    public static void Init(Entity spriteSheetEntity, SpriteSheetAnimator animator)
    {
        SpriteSheetAnimationScriptable startAnim = animator.animations[animator.defaultAnimationIndex];
        SpriteSheetCache.Instance.entityAnimator.Add(spriteSheetEntity, animator);
        SetAnimation(spriteSheetEntity, startAnim, false, false);
    }

    public static void SetAnimation(Entity e, int animationIndex, bool keepProgress = false)
    {
        var animator = SpriteSheetCache.Instance.GetAnimator(e);
        SetAnimation(e, animator.animations[animationIndex], keepProgress);
    }

    public static void SetAnimation(Entity e, string animationName, bool keepProgress = false)
    {
        var animator = SpriteSheetCache.Instance.GetAnimator(e);
        int animationIndex = animator.GetAnimationIndex(animationName);
        SetAnimation(e, animator.animations[animationIndex], keepProgress);
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

    public static void DestroyEntity(Entity e, string materialName)
    {
        Material material = SpriteSheetCache.Instance.GetMaterial(materialName);
        int bufferID = EntityManager.GetComponentData<BufferHook>(e).bufferID;
        DynamicBufferManager.RemoveBuffer(material, bufferID);
        EntityManager.DestroyEntity(e);
    }

    public static void DestroyEntity(EntityCommandBuffer commandBuffer, Entity e, BufferHook hook)
    {
        commandBuffer.DestroyEntity(e);
        Material material = DynamicBufferManager.GetMaterial(hook.bufferEnityID);
        DynamicBufferManager.RemoveBuffer(material, hook.bufferID);
    }

    public static void RecordSpriteSheet(Sprite[] sprites, string spriteSheetName, int spriteCount = 0)
    {
        KeyValuePair<Material, float4[]> atlasData = SpriteSheetCache.Instance.BakeSprites(sprites, spriteSheetName);
        DynamicBufferManager.GenerateBuffers(atlasData.Key, spriteCount);
        DynamicBufferManager.BakeUvBuffer(atlasData.Key, atlasData);
        renderInformation.Add(new RenderInformation(atlasData.Key, DynamicBufferManager.GetEntityBuffer(atlasData.Key)));
    }

    public static void RecordAnimator(SpriteSheetAnimator animator)
    {
        for (var i = 0; i < animator.animations.Length; i++)
        {
            var animation = animator.animations[i];
            animation.Init(i);
            var atlasData = SpriteSheetCache.Instance.BakeSprites(animation.sprites, animation.animationName);
            DynamicBufferManager.GenerateBuffers(atlasData.Key);
            DynamicBufferManager.BakeUvBuffer(atlasData.Key, atlasData);
            renderInformation.Add(new RenderInformation(atlasData.Key, DynamicBufferManager.GetEntityBuffer(atlasData.Key)));
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

    private static void SetAnimation(Entity e, SpriteSheetAnimationScriptable animation, bool keepProgress = false,
        bool cleanOldBuffer = true)
    {
        int bufferEnityID = EntityManager.GetComponentData<BufferHook>(e).bufferEnityID;
        int bufferID = EntityManager.GetComponentData<BufferHook>(e).bufferID;
        Material oldMaterial = DynamicBufferManager.GetMaterial(bufferEnityID);
        string oldAnimation = SpriteSheetCache.Instance.GetMaterialName(oldMaterial);

        Material material = SpriteSheetCache.Instance.GetMaterial(animation.animationName);

        if (cleanOldBuffer)
            DynamicBufferManager.RemoveBuffer(oldMaterial, bufferID);

        //use new buffer
        bufferID = DynamicBufferManager.AddDynamicBuffers(DynamicBufferManager.GetEntityBuffer(material), material);
        BufferHook bh = new BufferHook {bufferID = bufferID, bufferEnityID = DynamicBufferManager.GetEntityBufferID(material)};

        EntityManager.SetComponentData(e, bh);

        var animDefCmp = EntityManager.GetComponentData<SpriteSheetAnimationDefinitionComponent>(animation.definitionEntity);

        var animCmp = EntityManager.GetComponentData<SpriteSheetAnimationComponent>(e);
        if (keepProgress)
        {
            var currentAnimDefCmp = EntityManager.GetComponentData<SpriteSheetAnimationDefinitionComponent>(animCmp.CurrentAnimation);
            Debug.Assert(currentAnimDefCmp.maxSprites == animDefCmp.maxSprites);
            Debug.Assert(Mathf.Approximately(currentAnimDefCmp.duration, animDefCmp.duration));
        }
        else
        {
            EntityManager.SetComponentData(e, new SpriteIndex {Value = animDefCmp.startIndex});
            animCmp.FrameStartTime = Time.realtimeSinceStartup;
        }

        animCmp.IsPlaying = true;
        animCmp.CurrentAnimation = animation.definitionEntity;

        EntityManager.SetComponentData(e, animCmp);
    }
}