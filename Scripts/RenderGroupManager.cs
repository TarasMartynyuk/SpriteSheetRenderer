using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using System;
using System.Linq;
using Unity.Collections;

public static class RenderGroupManager
{
    static EntityManager EntityManager => World.DefaultGameObjectInjectionWorld.EntityManager;

    public static Entity CreateRenderGroup(Material material, float4[] uvs)
    {
        var renderGroup = CreateAnimationRenderGroup(material);
        UvBuffer.GetUV(renderGroup).CopyFrom(uvs);
        AddEntryToBuffers(renderGroup);
        return renderGroup;
    }

    public static int AddToGroup(Entity spritesheetRenderGroup)
    {
        AddEntryToBuffers(spritesheetRenderGroup);
        return EntityManager.GetBuffer<SpriteIndexBuffer>(spritesheetRenderGroup)[^1];
    }


    public static void RemoveFromRenderGroup(Entity spritesheetRenderGroup, int bufferID)
    {
        EntityManager.GetBuffer<SpriteIndexBuffer>(spritesheetRenderGroup).RemoveAtSwapBack(bufferID);
        EntityManager.GetBuffer<MatrixBuffer>(spritesheetRenderGroup).RemoveAtSwapBack(bufferID);
        EntityManager.GetBuffer<SpriteColorBuffer>(spritesheetRenderGroup).RemoveAtSwapBack(bufferID);
    }

    public static void RemoveFromRenderGroup(SpriteSheetRenderGroupHookComponent groupHookCmp) =>
        RemoveFromRenderGroup(groupHookCmp.SpritesheetRenderGroup, groupHookCmp.IndexInRenderGroup);

    private static void AddEntryToBuffers(Entity bufferEntity)
    {
        EntityManager.GetBuffer<SpriteIndexBuffer>(bufferEntity).Add(new SpriteIndexBuffer());
        EntityManager.GetBuffer<MatrixBuffer>(bufferEntity).Add(new MatrixBuffer());
        EntityManager.GetBuffer<SpriteColorBuffer>(bufferEntity).Add(new SpriteColorBuffer());
    }

    //use this when it's the first time you are using that material
    //use this just to generate the buffers container
    private static Entity CreateAnimationRenderGroup(Material material)
    {
        var archetype = EntityManager.CreateArchetype(
            typeof(SpriteIndexBuffer),
            typeof(MatrixBuffer),
            typeof(SpriteColorBuffer),
            typeof(Material),
            typeof(UvBuffer),
            typeof(SpriteSheetAnimationDefinitionComponent)
        );
        Entity renderGroup = EntityManager.CreateEntity(archetype);
        return renderGroup;
    }

}