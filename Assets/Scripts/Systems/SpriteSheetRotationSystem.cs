using Unity.Entities;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;

public class SpriteSheetRotationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref SpriteMatrix renderData, in Rotation2D rotation) =>
        {
            renderData.matrix.z = rotation.angle;
        }).ScheduleParallel();
    }


    //[BurstCompile]
    //struct SpriteSheetRotationJob : IJobForEach<Rotation2D, SpriteMatrix>
    //{
    //    public void Execute([ReadOnly] ref Rotation2D rotation, ref SpriteMatrix renderData)
    //    {
    //        renderData.matrix.z = rotation.angle;
    //    }
    //}

    //protected override JobHandle OnUpdate(JobHandle inputDeps)
    //{
    //    var job = new SpriteSheetRotationJob() { };
    //    return job.Schedule(this, inputDeps);
    //}
}
