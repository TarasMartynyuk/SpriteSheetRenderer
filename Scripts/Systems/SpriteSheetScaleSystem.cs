using Unity.Entities;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using Unity.Transforms;

public class SpriteSheetScaleSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref SpriteMatrix renderData, in Scale scale) =>
        {
            renderData.matrix.w = scale.Value;
        })
        .WithChangeFilter<Scale>()
        .Schedule();
    }


    //[BurstCompile]
    //struct SpriteSheetScaleJob : IJobForEach<Scale, SpriteMatrix>
    //{
    //    public void Execute([ReadOnly][ChangedFilter] ref Scale scale, ref SpriteMatrix renderData)
    //    {
    //        renderData.matrix.w = scale.Value;
    //    }
    //}

    //protected override JobHandle OnUpdate(JobHandle inputDeps)
    //{
    //    var job = new SpriteSheetScaleJob() { };
    //    return job.Schedule(this, inputDeps);
    //}
}
