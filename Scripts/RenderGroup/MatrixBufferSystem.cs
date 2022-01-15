using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

public class MatrixBufferSystem : SystemBase
{
    protected override void OnUpdate()
    {

        Entities.ForEach((Entity e, in SpriteSheetRenderGroupHookComponent hook, in LocalToWorld localToWorld) =>
        {
            var buffer = GetBuffer<MatrixBuffer>(hook.SpritesheetRenderGroup);
            buffer[hook.IndexInRenderGroup] = localToWorld.Value;
        })
        .WithChangeFilter<LocalToWorld>()
        .Schedule();
    }
}