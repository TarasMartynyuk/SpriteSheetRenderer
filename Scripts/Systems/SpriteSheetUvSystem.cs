using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public class SpriteSheetUvJobSystem : SystemBase
{
    //[BurstCompile]
    //struct UpdateJob : IJobForEach<SpriteIndex, BufferHook>
    //{
    //    [NativeDisableParallelForRestriction]
    //    public DynamicBuffer<SpriteIndexBuffer> indexBuffer;
    //    [ReadOnly]
    //    public int bufferEnityID;
    //    public void Execute([ReadOnly, ChangedFilter] ref SpriteIndex data, [ReadOnly] ref BufferHook hook)
    //    {
    //        if (bufferEnityID == hook.bufferEnityID)
    //            indexBuffer[hook.bufferID] = data.Value;
    //    }
    //}

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
            //.WithBurst()
            .Schedule();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        m_bufferEntities.Dispose();
    }
}
