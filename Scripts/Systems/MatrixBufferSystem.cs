using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class MatrixBufferSystem : SystemBase
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

        Entities.ForEach((in BufferHook hook, in LocalToWorld localToWorld) =>
            {
                var buffer = GetBuffer<MatrixBuffer>(bufferEntities[hook.bufferEnityID]);

                //var m = new float3x2 { c0 = data.matrix.xyz, c1 = new float3(5, -1, 1)};// * data.matrix.w };
                ////m[1][1] = m[1][1] * 2;
                //Debug.Log($"data.matrix: {data.matrix}");
                //Debug.Log($"float3x2: {m}");
                float rotation = ((Quaternion) localToWorld.Rotation).eulerAngles.z;

                Debug.Log($"MatrixBufferSystem rot: {rotation}");
                float scaleX = math.length(localToWorld.Value.c0.xyz);
                buffer[hook.bufferID] = new float4(localToWorld.Position.xy, rotation, scaleX);
            })
            .WithReadOnly(bufferEntities)
            //.WithChangeFilter<SpriteMatrix>()
            //.Schedule();
            .WithoutBurst()
            .Run();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        m_bufferEntities.Dispose();
    }
}