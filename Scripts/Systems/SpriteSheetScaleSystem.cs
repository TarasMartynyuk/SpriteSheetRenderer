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
}
