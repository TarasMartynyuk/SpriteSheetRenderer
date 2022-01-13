using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public class ColorBufferSystem : SystemBase
{
    NativeList<Entity> m_bufferEntities;

    protected override void OnCreate()
    {
        base.OnCreate();
        m_bufferEntities = new NativeList<Entity>(50, Allocator.Persistent);
    }

    protected override void OnUpdate()
    {
        RenderGroupManager.CopyBufferEntities(m_bufferEntities);
        var bufferEntities = m_bufferEntities.AsArray();
        Entities.ForEach((in SpriteSheetRenderGroupHookComponent hook, in SpriteSheetColor data) =>
            {
                var buffer = GetBuffer<SpriteColorBuffer>(bufferEntities[hook.IndexInRenderGroup]);
                buffer[hook.IndexInRenderGroup] = data.color;
            })
            .WithReadOnly(bufferEntities)
            //.WithChangeFilter<SpriteSheetColor>()
            .Schedule();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        m_bufferEntities.Dispose();
    }
}

public static class JobSystemUtils
{
    public static NativeArray<T> SingleElementInputArray<T>(T element, Allocator allocator = Allocator.TempJob) where T : struct
    {
        var result = new NativeArray<T>(1, allocator);
        result[0] = element;
        return result;
    }
}
