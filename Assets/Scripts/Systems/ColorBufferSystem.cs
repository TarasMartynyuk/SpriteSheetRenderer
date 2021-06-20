using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public class ColorBufferSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var buffers = DynamicBufferManager.GetColorBuffers();
        NativeArray<JobHandle> jobs = new NativeArray<JobHandle>(buffers.Length, Allocator.TempJob);

        //var bufferEnityIDInputArray = new NativeArray<int>

        for (int i = 0; i < buffers.Length; i++)
        {
            //var inputDeps = new UpdateJob()
            //{
            //    indexBuffer = buffers[i],
            //    bufferEnityID = i
            //}.Schedule(this, inputDeps);
            var indexBuffer = buffers[i].AsNativeArray();
            var bufferEnityID = i;

            //var inputDeps = 
            Entities.ForEach((ref SpriteSheetColor data, ref BufferHook hook) =>
            {
                if (bufferEnityID == hook.bufferEnityID)
                    indexBuffer[hook.bufferID] = data.color;
            })
            .Run();
            //.Schedule(Dependency);


            //jobs[i] = inputDeps;
        }

        //Entities.ForEach((ref SpriteSheetColor data, ref BufferHook hook) =>
        //    {
        //        //if (bufferEnityID == hook.bufferEnityID)
        //        //    indexBuffer[hook.bufferID] = data.color;

        //        var buffer = buffers[hook.bufferEnityID];
        //        buffer[hook.bufferID] = data.color;
        //    })
        //.WithBurst()
        //Dependency = JobHandle.CombineDependencies(jobs);
        //JobHandle.CompleteAll(jobs);
        jobs.Dispose();
    }


    //[BurstCompile]
    //struct UpdateJob : IJobForEach<SpriteSheetColor, BufferHook>
    //{
    //    [NativeDisableParallelForRestriction]
    //    public DynamicBuffer<SpriteColorBuffer> indexBuffer;
    //    [ReadOnly]
    //    public int bufferEnityID;
    //    public void Execute([ReadOnly, ChangedFilter] ref SpriteSheetColor data, [ReadOnly] ref BufferHook hook)
    //    {
    //        if (bufferEnityID == hook.bufferEnityID)
    //            indexBuffer[hook.bufferID] = data.color;
    //    }
    //}

    //protected override JobHandle OnUpdate(JobHandle inputDeps)
    //{
    //    var buffers = DynamicBufferManager.GetColorBuffers();
    //    NativeArray<JobHandle> jobs = new NativeArray<JobHandle>(buffers.Length, Allocator.TempJob);
    //    for (int i = 0; i < buffers.Length; i++)
    //    {
    //        inputDeps = new UpdateJob()
    //        {
    //            indexBuffer = buffers[i],
    //            bufferEnityID = i
    //        }.Schedule(this, inputDeps);
    //        jobs[i] = inputDeps;
    //    }
    //    JobHandle.CompleteAll(jobs);
    //    jobs.Dispose();
    //    return inputDeps;
    //}
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
