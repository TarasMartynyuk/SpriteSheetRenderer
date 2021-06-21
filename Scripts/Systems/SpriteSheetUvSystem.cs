using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public class SpriteSheetUvJobSystem : SystemBase
{
    NativeList<Entity> m_bufferEntities;

    protected override void OnCreate()
    {
        base.OnCreate();
        m_bufferEntities = new NativeList<Entity>(50, Allocator.Persistent);
    }

    protected override void OnUpdate()
    {
        DynamicBufferManager.CopyBufferEntities(m_bufferEntities);
        var bufferEntities = m_bufferEntities.AsArray();
        var entityManager = EntityManager;

        Entities.ForEach((in BufferHook hook, in SpriteIndex data) =>
            {
                var buffer = GetBuffer<SpriteIndexBuffer>(bufferEntities[hook.bufferEnityID]);
                buffer[hook.bufferID] = data.Value;
            })
            .WithReadOnly(bufferEntities)
            .WithChangeFilter<SpriteIndex>()
            .Schedule();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        m_bufferEntities.Dispose();
    }
}
