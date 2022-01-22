using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class SpriteSheetManager : SingletonBase<SpriteSheetManager>
{
    public List<RenderInformation> RenderInformation { get; } = new();

    static EntityManager EntityManager => World.DefaultGameObjectInjectionWorld.EntityManager;

    public static void SetAnimation(Entity e, Entity animationRenderGroup, bool keepProgress = false) =>
        SetAnimationInternal(e, animationRenderGroup, EntityManager, keepProgress);

    public static void ChangeRenderGroup(Entity entity, Entity animationRenderGroup, EntityManager eManager)
    {
        var groupHookCmp = eManager.GetComponentData<SpriteSheetRenderGroupHookComponent>(entity);

        if (groupHookCmp.SpritesheetRenderGroup != Entity.Null)
            RenderGroupManager.RemoveFromRenderGroup(entity);

        int indexInGroup = RenderGroupManager.AddToGroup(animationRenderGroup, entity);
        groupHookCmp = new SpriteSheetRenderGroupHookComponent
            {IndexInRenderGroup = indexInGroup, SpritesheetRenderGroup = animationRenderGroup};

        eManager.SetComponentData(entity, groupHookCmp);
    }

    public void Init(Shader spriteSheetShader)
    {
        SpriteSheetCache.Instance.Init(spriteSheetShader);
        EntityManager.CreateEntity("SpriteSheetAnimationSingleton", typeof(AnimationChangeCommandBufferElement));
    }

    public void RecordAnimator(SpriteSheetAnimator animator)
    {
        foreach (var animation in animator.animations)
        {
            var atlasData = SpriteSheetCache.Instance.BakeSprites(animation.Sprites, animation.AnimationName);
            var newRenderGroup = RenderGroupManager.CreateRenderGroup(atlasData.Value, animation.AnimationName);
            animation.Init(newRenderGroup);

            RenderInformation.Add(new RenderInformation(atlasData.Key, newRenderGroup));
        }
    }

    public void RecordStaticSprite(StaticSpriteScriptable sprite)
    {
        var atlasData = SpriteSheetCache.Instance.BakeSprite(sprite.Sprite, sprite.name);
        var renderGroup = RenderGroupManager.CreateRenderGroup(atlasData.Value, sprite.name);
        sprite.Init(renderGroup);
        RenderInformation.Add(new RenderInformation(atlasData.Key, renderGroup));
    }

    public void DestroyEntity(Entity e)
    {
        RenderGroupManager.RemoveFromRenderGroup(e);
        EntityManager.DestroyEntity(e);
    }

    public void DestroyEntity(EntityCommandBuffer commandBuffer, Entity e, SpriteSheetRenderGroupHookComponent hook)
    {
        throw new NotImplementedException();
        // commandBuffer.DestroyEntity(e);
        // Material material = DynamicBufferManager.GetMaterial(hook.bufferEnityID);
        // DynamicBufferManager.RemoveBuffer(material, hook.bufferID);
    }


    public void CleanBuffers()
    {
        for (int i = 0; i < RenderInformation.Count; i++)
            RenderInformation[i].DestroyBuffers();
        RenderInformation.Clear();
    }

    public void ReleaseUvBuffer(int bufferID)
    {
        if (RenderInformation[bufferID].uvBuffer != null)
            RenderInformation[bufferID].uvBuffer.Release();
    }

    public void ReleaseBuffer(int bufferID)
    {
        if (RenderInformation[bufferID].matrixBuffer != null)
            RenderInformation[bufferID].matrixBuffer.Release();
        if (RenderInformation[bufferID].colorsBuffer != null)
            RenderInformation[bufferID].colorsBuffer.Release();
        // if(RenderInformation[bufferID].uvBuffer != null)
        //     RenderInformation[bufferID].uvBuffer.Release();
        if (RenderInformation[bufferID].indexBuffer != null)
            RenderInformation[bufferID].indexBuffer.Release();
    }

    private static void SetAnimationInternal(Entity entity, Entity animationRenderGroup, EntityManager eManager, bool keepProgress = false)
    {
        ChangeRenderGroup(entity, animationRenderGroup, eManager);

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
            animCmp.FrameStartTime = Time.realtimeSinceStartup;
        }

        animCmp.Status = ESpriteSheetAnimationStatus.Playing;
        animCmp.CurrentAnimation = animationRenderGroup;

        eManager.SetComponentData(entity, animCmp);

        // DebugExtensions.LogVar(new { e = entity.Stringify(), anim = animationRenderGroup.Stringify() }, "Anim change", addCurrFrame:true);
    }
}