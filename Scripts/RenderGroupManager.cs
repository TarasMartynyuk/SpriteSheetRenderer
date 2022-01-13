using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using System;
using System.Linq;
using Unity.Collections;

//todo entity is a dictionary with spritesheetmaterial and is used to separate buffers from different material

public static class RenderGroupManager
{
    public static EntityManager EntityManager => World.DefaultGameObjectInjectionWorld.EntityManager;

    //list of all the "Enities with all the buffers"
    //Each different material have a different bufferEnity
    public static List<Entity> m_animationRenderGroups { get; } = new();

    //contains the index of a bufferEntity inside the bufferEntities from a material
    private static Dictionary<Material, Entity> m_materialToAnimationRenderGroup = new();

    private static Dictionary<Entity, List<int>> m_renderGroupToAvailableIndices = new();

    public static NativeArray<Entity> CopyBufferEntities(Allocator allocator = Allocator.TempJob)
    {
        var result = new NativeArray<Entity>(m_animationRenderGroups.Count, allocator);
        CopyBufferEntities(result);
        return result;
    }

    public static void CopyBufferEntities(NativeArray<Entity> outResult)
    {
        for (int i = 0; i < m_animationRenderGroups.Count; i++)
        {
            outResult[i] = m_animationRenderGroups[i];
        }
    }

    public static void CopyBufferEntities(NativeList<Entity> result)
    {
        result.Capacity = m_animationRenderGroups.Count;
        result.ResizeUninitialized(m_animationRenderGroups.Count);
        CopyBufferEntities(result.AsArray());
    }


    //only use this when you didn't bake the uv yet
    public static void BakeUvBuffer(Material spriteSheetMaterial, KeyValuePair<Material, float4[]> atlasData)
    {
        Entity entity = GerRenderGroup(spriteSheetMaterial);
        var buffer = EntityManager.GetBuffer<UvBuffer>(entity);
        for (int j = 0; j < atlasData.Value.Length; j++)
            buffer.Add(atlasData.Value[j]);
    }

    public static void GenerateBuffers(Material material, int entityCount = 0)
    {
        if (!m_materialToAnimationRenderGroup.ContainsKey(material))
        {
            CreateAnimationRenderGroup(material);
            m_renderGroupToAvailableIndices.Add(material, new List<int>());
            for (int i = 0; i < entityCount; i++)
                m_renderGroupToAvailableIndices[material].Add(i);
            MassAddBuffers(m_animationRenderGroups.Last(), entityCount);
        }
    }

    //use this when it's the first time you are using that material
    //use this just to generate the buffers container
    public static void CreateAnimationRenderGroup(Material material)
    {
        var archetype = EntityManager.CreateArchetype(
            typeof(SpriteIndexBuffer),
            typeof(MatrixBuffer),
            typeof(SpriteColorBuffer),
            typeof(Material),
            typeof(UvBuffer),
            typeof(SpriteSheetAnimationDefinitionComponent)
        );
        Entity bufferEntity = EntityManager.CreateEntity(archetype);
        m_animationRenderGroups.Add(bufferEntity);
        m_materialToAnimationRenderGroup.Add(material, bufferEntity);
    }

    //when u create a new entity you need a new buffer for him
    //use this to add new dynamicbuffer
    public static void MassAddBuffers(Entity bufferEntity, int entityCount)
    {
        var indexBuffer = EntityManager.GetBuffer<SpriteIndexBuffer>(bufferEntity);
        var colorBuffer = EntityManager.GetBuffer<SpriteColorBuffer>(bufferEntity);
        var matrixBuffer = EntityManager.GetBuffer<MatrixBuffer>(bufferEntity);
        for (int i = 0; i < entityCount; i++)
        {
            indexBuffer.Add(new SpriteIndexBuffer());
            matrixBuffer.Add(new MatrixBuffer());
            colorBuffer.Add(new SpriteColorBuffer());
        }
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

    // public static BufferHook GetBufferHook(Material  material)
    // {
    //     return new BufferHook { bufferEnityID = materialToBuffersEntityIndex[material], bufferID = NextIDForEntity(material) };
    // }

    // public static int GetEntityBufferID(Material  material)
    // {
    //     return materialToBuffersEntity[material];
    // }

    public static Entity GerRenderGroup(Material material)
    {
        return m_materialToAnimationRenderGroup[material]; //bufferEntities[materialToBuffersEntity[material]];
    }

    public static int NextIDForEntity(Entity spritesheetRenderGroup)
    {
        var ids = m_renderGroupToAvailableIndices[spritesheetRenderGroup];
        var availableIds = Enumerable.Range(0, ids.Count + 1).Except(ids);
        int smallerID = availableIds.First();
        ids.Add(smallerID);
        return smallerID;
    }
    // public static Material GetMaterial(int bufferEntityID)
    // {
    //     foreach (KeyValuePair<Material, int> e in materialToBuffersEntity)
    //         if (e.Value == bufferEntityID)
    //             return e.Key;
    //     return null;
    // }

    public static Material GetMaterial(Entity bufferEntity)
    {
        foreach (var pair in m_materialToAnimationRenderGroup)
            if (pair.Value == bufferEntity)
                return pair.Key;
        return null;
    }

    // public static void RemoveBuffer(Material material, int bufferID)
    // {
    //     Entity bufferEntity = GetEntityBuffer(material);
    //     availableEntityID[material].Remove(bufferID);
    //     CleanBuffer(bufferID, bufferEntity);
    // }

    public static void RemoveFromRenderGroup(Entity spritesheetRenderGroup, int bufferID)
    {
        m_renderGroupToAvailableIndices[spritesheetRenderGroup].Remove(bufferID);
        CleanBuffer(bufferID, spritesheetRenderGroup);
    }

    public static void RemoveFromRenderGroup(SpriteSheetRenderGroupHookComponent groupHookCmp) =>
        RemoveFromRenderGroup(groupHookCmp.SpritesheetRenderGroup, groupHookCmp.IndexInRenderGroup);

    private static void CleanBuffer(int bufferID, Entity spritesheetRenderGroup)
    {
        EntityManager.GetBuffer<SpriteIndexBuffer>(spritesheetRenderGroup).RemoveAt(bufferID);
        EntityManager.GetBuffer<MatrixBuffer>(spritesheetRenderGroup).RemoveAt(bufferID);
        EntityManager.GetBuffer<SpriteColorBuffer>(spritesheetRenderGroup).RemoveAt(bufferID);

        EntityManager.GetBuffer<SpriteIndexBuffer>(spritesheetRenderGroup).Insert(bufferID, new SpriteIndexBuffer {index = -1});
        EntityManager.GetBuffer<MatrixBuffer>(spritesheetRenderGroup).Insert(bufferID, new MatrixBuffer());
        EntityManager.GetBuffer<SpriteColorBuffer>(spritesheetRenderGroup).Insert(bufferID, new SpriteColorBuffer());
    }

    public static DynamicBuffer<SpriteIndexBuffer>[] GetIndexBuffers()
    {
        DynamicBuffer<SpriteIndexBuffer>[] buffers = new DynamicBuffer<SpriteIndexBuffer>[m_animationRenderGroups.Count];
        for (int i = 0; i < buffers.Length; i++)
            buffers[i] = EntityManager.GetBuffer<SpriteIndexBuffer>(m_animationRenderGroups[i]);
        return buffers;
    }

    public static DynamicBuffer<MatrixBuffer>[] GetMatrixBuffers()
    {
        DynamicBuffer<MatrixBuffer>[] buffers = new DynamicBuffer<MatrixBuffer>[m_animationRenderGroups.Count];
        for (int i = 0; i < buffers.Length; i++)
            buffers[i] = EntityManager.GetBuffer<MatrixBuffer>(m_animationRenderGroups[i]);
        return buffers;
    }

    public static DynamicBuffer<SpriteColorBuffer>[] GetColorBuffers()
    {
        DynamicBuffer<SpriteColorBuffer>[] buffers = new DynamicBuffer<SpriteColorBuffer>[m_animationRenderGroups.Count];
        for (int i = 0; i < buffers.Length; i++)
            buffers[i] = EntityManager.GetBuffer<SpriteColorBuffer>(m_animationRenderGroups[i]);
        return buffers;
    }

    public static DynamicBuffer<UvBuffer>[] GetUvBuffers()
    {
        DynamicBuffer<UvBuffer>[] buffers = new DynamicBuffer<UvBuffer>[m_animationRenderGroups.Count];
        for (int i = 0; i < buffers.Length; i++)
            buffers[i] = EntityManager.GetBuffer<UvBuffer>(m_animationRenderGroups[i]);
        return buffers;
    }
}