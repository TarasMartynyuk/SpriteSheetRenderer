using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

public partial class MatrixBufferSystem : SystemBase
{
    protected override void OnUpdate()
    {

        Entities.ForEach((in SpriteSheetRenderGroupHookComponent hook, in LocalToWorld localToWorld) =>
        {
            var buffer = GetBuffer<MatrixBuffer>(hook.SpritesheetRenderGroup);
            buffer[hook.IndexInRenderGroup] = localToWorld.Value;
        })
        .Schedule();
    }
}