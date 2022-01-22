using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using System;
using System.Linq;
using Unity.Collections;
using Unity.Transforms;
using UnityEditor;

public static class RenderGroupManager
{
    static EntityManager EntityManager => World.DefaultGameObjectInjectionWorld.EntityManager;

    public static Entity CreateRenderGroup(float4[] uvs, string name)
    {
        var archetype = EntityManager.CreateArchetype(
            typeof(SpriteIndexBuffer),
            typeof(MatrixBuffer),
            typeof(SpriteColorBufferElement),
            typeof(Material),
            typeof(UvBuffer),
            typeof(RenderedEntityBufferElement),
            typeof(SpriteSheetAnimationDefinitionComponent)
        );
        Entity renderGroup = EntityManager.CreateEntity(archetype);

#if UNITY_EDITOR
        EntityManager.SetNameIndexed(renderGroup, name);
#endif

        UvBuffer.GetUV(renderGroup).CopyFrom(uvs);
        // AddEntryToBuffers(renderGroup);
        return renderGroup;
    }

    public static int AddToGroup(Entity spritesheetRenderGroup, Entity entity)
    {
        AssertBuffersSameLength(spritesheetRenderGroup);

        RenderedEntityBufferElement.GetRenderedEntities(spritesheetRenderGroup).Add(entity);
        SpriteIndexBuffer.GetSpriteIndices(spritesheetRenderGroup).Add(EntityManager.GetComponentData<SpriteIndex>(entity).Value);
        MatrixBuffer.GetMatrixBuffer(spritesheetRenderGroup).Add(EntityManager.GetComponentData<LocalToWorld>(entity).Value);
        SpriteColorBufferElement.GetColors(spritesheetRenderGroup).Add(EntityManager.GetComponentData<SpriteSheetColor>(entity).Value);

        return RenderedEntityBufferElement.GetRenderedEntities(spritesheetRenderGroup).LastIndex();
    }

    public static void RemoveFromRenderGroup(Entity entity)
    {
        using var hookCmpRef = new ComponentReference<SpriteSheetRenderGroupHookComponent>(entity);
        ref var hookCmp = ref hookCmpRef.Value();
        AssertBuffersSameLength(hookCmp.SpritesheetRenderGroup);
        
        var renderedEntities = RenderedEntityBufferElement.GetRenderedEntities(hookCmp.SpritesheetRenderGroup);
        var swapped = renderedEntities[^1];
        using (var swappedHookCmpRef = new ComponentReference<SpriteSheetRenderGroupHookComponent>(swapped))
            swappedHookCmpRef.Value().IndexInRenderGroup = hookCmp.IndexInRenderGroup;

        renderedEntities.RemoveAtSwapBack(hookCmp.IndexInRenderGroup);
        EntityManager.GetBuffer<SpriteIndexBuffer>(hookCmp.SpritesheetRenderGroup).RemoveAtSwapBack(hookCmp.IndexInRenderGroup);
        EntityManager.GetBuffer<MatrixBuffer>(hookCmp.SpritesheetRenderGroup).RemoveAtSwapBack(hookCmp.IndexInRenderGroup);
        EntityManager.GetBuffer<SpriteColorBufferElement>(hookCmp.SpritesheetRenderGroup).RemoveAtSwapBack(hookCmp.IndexInRenderGroup);
        
        hookCmp.SpritesheetRenderGroup = Entity.Null;
    }

    private static int GetLength<T>(DynamicBuffer<T> b)
        where T : struct => b.Length;

    private static void AssertBuffersSameLength(Entity renderGroup)
    {
        var renderedEntities = RenderedEntityBufferElement.GetRenderedEntities(renderGroup);
        var spriteIndices = SpriteIndexBuffer.GetSpriteIndices(renderGroup);
        var matrices = MatrixBuffer.GetMatrixBuffer(renderGroup);
        var colors = SpriteColorBufferElement.GetColors(renderGroup);
        
        int length = GetLength(renderedEntities);
        Debug.Assert(GetLength(spriteIndices) == length &&
                     GetLength(matrices) == length &&
                     GetLength(colors) == length);
    }
}