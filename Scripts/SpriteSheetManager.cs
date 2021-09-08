﻿using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public abstract class SpriteSheetManager
{
    public static List<RenderInformation> renderInformation = new List<RenderInformation>();

    public static EntityManager EntityManager => World.DefaultGameObjectInjectionWorld.EntityManager;

    public static Entity Instantiate(EntityArchetype archetype, string spriteSheetName)
    {
        Entity e = EntityManager.CreateEntity(archetype);
        Material material = SpriteSheetCache.Instance.GetMaterial(spriteSheetName);
        int bufferID = DynamicBufferManager.AddDynamicBuffers(DynamicBufferManager.GetEntityBuffer(material), material);

        var spriteSheetMaterial = new SpriteSheetMaterial { material = material };
        BufferHook bh = new BufferHook { bufferID = bufferID, bufferEnityID = DynamicBufferManager.GetEntityBufferID(spriteSheetMaterial) };
        EntityManager.SetComponentData(e, bh);
        EntityManager.SetSharedComponentData(e, spriteSheetMaterial);
        return e;
    }

    public static Entity Instantiate(EntityArchetype archetype, SpriteSheetAnimator animator)
    {
        Entity e = EntityManager.CreateEntity(archetype);
        animator.currentAnimationIndex = animator.defaultAnimationIndex;
        SpriteSheetAnimationData startAnim = animator.animations[animator.defaultAnimationIndex];
        int maxSprites = startAnim.sprites.Length;
        Material material = SpriteSheetCache.Instance.GetMaterial(animator.animations[animator.defaultAnimationIndex].animationName);
        int bufferID = DynamicBufferManager.AddDynamicBuffers(DynamicBufferManager.GetEntityBuffer(material), material);

        var spriteSheetMaterial = new SpriteSheetMaterial { material = material };
        BufferHook bh = new BufferHook { bufferID = bufferID, bufferEnityID = DynamicBufferManager.GetEntityBufferID(spriteSheetMaterial) };
        EntityManager.SetComponentData(e, bh);
        EntityManager.SetComponentData(e, new SpriteSheetAnimation { maxSprites = maxSprites, play = startAnim.playOnStart, samples = startAnim.samples, repetition = startAnim.repetition });
        EntityManager.SetComponentData(e, new SpriteIndex { Value = startAnim.startIndex });
        EntityManager.SetSharedComponentData(e, spriteSheetMaterial);
        animator.managedEntity = e;
        SpriteSheetCache.Instance.entityAnimator.Add(e, animator);
        return e;
    }

    public static void SetAnimation(Entity e, SpriteSheetAnimationData animation)
    {
        int bufferEnityID = EntityManager.GetComponentData<BufferHook>(e).bufferEnityID;
        int bufferID = EntityManager.GetComponentData<BufferHook>(e).bufferID;
        Material oldMaterial = DynamicBufferManager.GetMaterial(bufferEnityID);
        string oldAnimation = SpriteSheetCache.Instance.GetMaterialName(oldMaterial);
        if (animation.animationName != oldAnimation)
        {
            Material material = SpriteSheetCache.Instance.GetMaterial(animation.animationName);
            var spriteSheetMaterial = new SpriteSheetMaterial { material = material };

            DynamicBufferManager.RemoveBuffer(oldMaterial, bufferID);

            //use new buffer
            bufferID = DynamicBufferManager.AddDynamicBuffers(DynamicBufferManager.GetEntityBuffer(material), material);
            BufferHook bh = new BufferHook { bufferID = bufferID, bufferEnityID = DynamicBufferManager.GetEntityBufferID(spriteSheetMaterial) };

            EntityManager.SetSharedComponentData(e, spriteSheetMaterial);
            EntityManager.SetComponentData(e, bh);
        }
        EntityManager.SetComponentData(e, new SpriteSheetAnimation { maxSprites = animation.sprites.Length, play = animation.playOnStart, samples = animation.samples, repetition = animation.repetition, elapsedFrames = 0 });
        EntityManager.SetComponentData(e, new SpriteIndex { Value = animation.startIndex });
    }

    public static void SetAnimation(EntityCommandBuffer commandBuffer, Entity e, SpriteSheetAnimationData animation, BufferHook hook)
    {
        Material oldMaterial = DynamicBufferManager.GetMaterial(hook.bufferEnityID);
        string oldAnimation = SpriteSheetCache.Instance.GetMaterialName(oldMaterial);
        if (animation.animationName != oldAnimation)
        {
            Material material = SpriteSheetCache.Instance.GetMaterial(animation.animationName);
            var spriteSheetMaterial = new SpriteSheetMaterial { material = material };

            //clean old buffer
            DynamicBufferManager.RemoveBuffer(oldMaterial, hook.bufferID);

            //use new buffer
            int bufferID = DynamicBufferManager.AddDynamicBuffers(DynamicBufferManager.GetEntityBuffer(material), material);
            BufferHook bh = new BufferHook { bufferID = bufferID, bufferEnityID = DynamicBufferManager.GetEntityBufferID(spriteSheetMaterial) };

            commandBuffer.SetSharedComponent(e, spriteSheetMaterial);
            commandBuffer.SetComponent(e, bh);
        }
        commandBuffer.SetComponent(e, new SpriteSheetAnimation { maxSprites = animation.sprites.Length, play = animation.playOnStart, samples = animation.samples, repetition = animation.repetition, elapsedFrames = 0 });
        commandBuffer.SetComponent(e, new SpriteIndex { Value = animation.startIndex });
    }

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
        SpriteSheetMaterial material = new SpriteSheetMaterial { material = atlasData.Key };
        DynamicBufferManager.GenerateBuffers(material, spriteCount);
        DynamicBufferManager.BakeUvBuffer(material, atlasData);
        renderInformation.Add(new RenderInformation(material.material, DynamicBufferManager.GetEntityBuffer(material.material)));
    }

    public static void RecordAnimator(SpriteSheetAnimator animator)
    {
        foreach (SpriteSheetAnimationData animation in animator.animations)
        {
            KeyValuePair<Material, float4[]> atlasData = SpriteSheetCache.Instance.BakeSprites(animation.sprites, animation.animationName);
            SpriteSheetMaterial material = new SpriteSheetMaterial { material = atlasData.Key };
            DynamicBufferManager.GenerateBuffers(material);
            DynamicBufferManager.BakeUvBuffer(material, atlasData);
            renderInformation.Add(new RenderInformation(material.material, DynamicBufferManager.GetEntityBuffer(material.material)));
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

}
