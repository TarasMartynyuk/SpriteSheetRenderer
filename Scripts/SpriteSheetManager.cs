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

        var spriteSheetMaterial = new SpriteSheetMaterial {material = material};
        BufferHook bh = new BufferHook {bufferID = bufferID, bufferEnityID = DynamicBufferManager.GetEntityBufferID(spriteSheetMaterial)};
        EntityManager.SetComponentData(e, bh);
        EntityManager.SetSharedComponentData(e, spriteSheetMaterial);
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
        SpriteSheetAnimationData startAnim = animator.animations[animator.defaultAnimationIndex];
        int maxSprites = startAnim.sprites.Length;
        Material material = SpriteSheetCache.Instance.GetMaterial(animator.animations[animator.defaultAnimationIndex].fullName);
        int bufferID = DynamicBufferManager.AddDynamicBuffers(DynamicBufferManager.GetEntityBuffer(material), material);

        var spriteSheetMaterial = new SpriteSheetMaterial {material = material};
        BufferHook bh = new BufferHook {bufferID = bufferID, bufferEnityID = DynamicBufferManager.GetEntityBufferID(spriteSheetMaterial)};
        EntityManager.SetComponentData(spriteSheetEntity, bh);
        EntityManager.SetComponentData(spriteSheetEntity,
            new SpriteSheetAnimationComponent
                {maxSprites = maxSprites, isPlaying = startAnim.playOnStart, frameDuration = startAnim.frameDuration, repetition = startAnim.repetition});
        EntityManager.SetComponentData(spriteSheetEntity, new SpriteIndex {Value = startAnim.startIndex});
        EntityManager.SetSharedComponentData(spriteSheetEntity, spriteSheetMaterial);
        SpriteSheetCache.Instance.entityAnimator.Add(spriteSheetEntity, animator);
    }

    public static void SetAnimation(Entity e, int animationIndex, bool keepProgress = false)
    {
        var animator = SpriteSheetCache.Instance.GetAnimator(e);
        SetAnimation(e, animator.animations[animationIndex], animationIndex, keepProgress);
    }

    public static void SetAnimation(Entity e, string animationName, bool keepProgress = false)
    {
        var animator = SpriteSheetCache.Instance.GetAnimator(e);
        int animationIndex = animator.GetAnimationIndex(animationName);
        SetAnimation(e, animator.animations[animationIndex], animationIndex, keepProgress);
    }

    public static void SetAnimation(EntityCommandBuffer commandBuffer, Entity e, SpriteSheetAnimationData animation, BufferHook hook)
    {
        Material oldMaterial = DynamicBufferManager.GetMaterial(hook.bufferEnityID);
        string oldAnimation = SpriteSheetCache.Instance.GetMaterialName(oldMaterial);
        if (animation.fullName != oldAnimation)
        {
            Material material = SpriteSheetCache.Instance.GetMaterial(animation.fullName);
            var spriteSheetMaterial = new SpriteSheetMaterial {material = material};

            //clean old buffer
            DynamicBufferManager.RemoveBuffer(oldMaterial, hook.bufferID);

            //use new buffer
            int bufferID = DynamicBufferManager.AddDynamicBuffers(DynamicBufferManager.GetEntityBuffer(material), material);
            BufferHook bh = new BufferHook {bufferID = bufferID, bufferEnityID = DynamicBufferManager.GetEntityBufferID(spriteSheetMaterial)};

            commandBuffer.SetSharedComponent(e, spriteSheetMaterial);
            commandBuffer.SetComponent(e, bh);
        }

        commandBuffer.SetComponent(e,
            new SpriteSheetAnimationComponent
            {
                maxSprites = animation.sprites.Length, isPlaying = animation.playOnStart, frameDuration = animation.frameDuration,
                repetition = animation.repetition
            });
        commandBuffer.SetComponent(e, new SpriteIndex {Value = animation.startIndex});
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
        SpriteSheetMaterial material = new SpriteSheetMaterial {material = atlasData.Key};
        DynamicBufferManager.GenerateBuffers(material, spriteCount);
        DynamicBufferManager.BakeUvBuffer(material, atlasData);
        renderInformation.Add(new RenderInformation(material.material, DynamicBufferManager.GetEntityBuffer(material.material)));
    }

    public static void RecordAnimator(SpriteSheetAnimator animator)
    {
        animator.Init();
        foreach (SpriteSheetAnimationData animation in animator.animations)
        {
            KeyValuePair<Material, float4[]> atlasData = SpriteSheetCache.Instance.BakeSprites(animation.sprites, animation.fullName);
            SpriteSheetMaterial material = new SpriteSheetMaterial {material = atlasData.Key};
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
    
    private static void SetAnimation(Entity e, SpriteSheetAnimationData animation, int animationIndex, bool keepProgress = false)
    {
        int bufferEnityID = EntityManager.GetComponentData<BufferHook>(e).bufferEnityID;
        int bufferID = EntityManager.GetComponentData<BufferHook>(e).bufferID;
        Material oldMaterial = DynamicBufferManager.GetMaterial(bufferEnityID);
        string oldAnimation = SpriteSheetCache.Instance.GetMaterialName(oldMaterial);
        if (animation.fullName != oldAnimation)
        {
            Material material = SpriteSheetCache.Instance.GetMaterial(animation.fullName);
            var spriteSheetMaterial = new SpriteSheetMaterial {material = material};

            DynamicBufferManager.RemoveBuffer(oldMaterial, bufferID);

            //use new buffer
            bufferID = DynamicBufferManager.AddDynamicBuffers(DynamicBufferManager.GetEntityBuffer(material), material);
            BufferHook bh = new BufferHook {bufferID = bufferID, bufferEnityID = DynamicBufferManager.GetEntityBufferID(spriteSheetMaterial)};

            EntityManager.SetSharedComponentData(e, spriteSheetMaterial);
            EntityManager.SetComponentData(e, bh);
        }

        var animCmp = EntityManager.GetComponentData<SpriteSheetAnimationComponent>(e);
        if (!keepProgress)
        {
            animCmp.maxSprites = animation.sprites.Length;
            animCmp.isPlaying = animation.playOnStart;
            animCmp.frameDuration = animation.frameDuration;
            
            EntityManager.SetComponentData(e, new SpriteIndex {Value = animation.startIndex});
        }
        else
        {
            Debug.Assert(animCmp.maxSprites == animation.sprites.Length);
            Debug.Assert(Mathf.Approximately(animCmp.frameDuration, animation.frameDuration));
        }
        
        animCmp.animationIndex = animationIndex;
        EntityManager.SetComponentData(e, animCmp);
    }
}