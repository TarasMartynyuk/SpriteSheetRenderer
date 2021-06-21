using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public class MatrixBufferSystem : SystemBase
{
    //[BurstCompile]
    //struct UpdateJob : IJobForEach<SpriteMatrix, BufferHook>
    //{
    //    [NativeDisableParallelForRestriction]
    //    public DynamicBuffer<MatrixBuffer> indexBuffer;
    //    [ReadOnly]
    //    public int bufferEnityID;
    //    public void Execute([ReadOnly, ChangedFilter] ref SpriteMatrix data, [ReadOnly] ref BufferHook hook)
    //    {
    //        if (bufferEnityID == hook.bufferEnityID)
    //            indexBuffer[hook.bufferID] = data.matrix;
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

        Entities.ForEach((in BufferHook hook, in SpriteMatrix data) =>
            {
                var buffer = GetBuffer<MatrixBuffer>(bufferEntities[hook.bufferEnityID]);
                buffer[hook.bufferID] = data.matrix;
            })
            .WithReadOnly(bufferEntities)
            .WithChangeFilter<SpriteMatrix>()
            //.WithBurst()
            .Schedule();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        m_bufferEntities.Dispose();
    }
}