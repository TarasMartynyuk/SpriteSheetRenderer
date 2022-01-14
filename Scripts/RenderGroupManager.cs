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


    static Dictionary<Entity, List<int>> m_renderGroupToAvailableIndices = new();

    //list of all the "Enities with all the buffers"
    //Each different material have a different bufferEnity

    public static Entity CreateRenderGroup(Material material, float4[] uvs)
    {
        var renderGroup = CreateAnimationRenderGroup(material);
        UvBuffer.GetUV(renderGroup).CopyFrom(uvs);
        MassAddBuffers(renderGroup);
        m_renderGroupToAvailableIndices.Add(renderGroup, new List<int>());
        return renderGroup;
    }

    public static int AddDynamicBuffers(Entity spritesheetRenderGroup)
    {
        int bufferId = NextIDForEntity(spritesheetRenderGroup);
        var indexBuffer = EntityManager.GetBuffer<SpriteIndexBuffer>(spritesheetRenderGroup);
        var colorBuffer = EntityManager.GetBuffer<SpriteColorBuffer>(spritesheetRenderGroup);
        var matrixBuffer = EntityManager.GetBuffer<MatrixBuffer>(spritesheetRenderGroup);
        if (indexBuffer.Length <= bufferId)
        {
            indexBuffer.Add(new SpriteIndexBuffer());
            colorBuffer.Add(new SpriteColorBuffer());
            matrixBuffer.Add(new MatrixBuffer());
        }

        return bufferId;
    }

    private static int NextIDForEntity(Entity spritesheetRenderGroup)
    {
        var ids = m_renderGroupToAvailableIndices[spritesheetRenderGroup];
        var availableIds = Enumerable.Range(0, ids.Count + 1).Except(ids);
        int smallerID = availableIds.First();
        ids.Add(smallerID);
        return smallerID;
    }

    public static void RemoveFromRenderGroup(Entity spritesheetRenderGroup, int bufferID)
    {
        m_renderGroupToAvailableIndices[spritesheetRenderGroup].Remove(bufferID);
        CleanBuffer(bufferID, spritesheetRenderGroup);
    }

    public static void RemoveFromRenderGroup(SpriteSheetRenderGroupHookComponent groupHookCmp) =>
        RemoveFromRenderGroup(groupHookCmp.SpritesheetRenderGroup, groupHookCmp.IndexInRenderGroup);

    //when u create a new entity you need a new buffer for him
    //use this to add new dynamicbuffer
    private static void MassAddBuffers(Entity bufferEntity)
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

    private static void CleanBuffer(int bufferID, Entity spritesheetRenderGroup)
    {
        EntityManager.GetBuffer<SpriteIndexBuffer>(spritesheetRenderGroup).RemoveAt(bufferID);
        EntityManager.GetBuffer<MatrixBuffer>(spritesheetRenderGroup).RemoveAt(bufferID);
        EntityManager.GetBuffer<SpriteColorBuffer>(spritesheetRenderGroup).RemoveAt(bufferID);

        EntityManager.GetBuffer<SpriteIndexBuffer>(spritesheetRenderGroup).Insert(bufferID, new SpriteIndexBuffer {index = -1});
        EntityManager.GetBuffer<MatrixBuffer>(spritesheetRenderGroup).Insert(bufferID, new MatrixBuffer());
        EntityManager.GetBuffer<SpriteColorBuffer>(spritesheetRenderGroup).Insert(bufferID, new SpriteColorBuffer());
    }
}