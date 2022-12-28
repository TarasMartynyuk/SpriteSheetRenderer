using SmokGnu.SpriteSheetRenderer.Animation.Components;
using SmokGnu.SpriteSheetRenderer.Render.RenderGroup.Components;
using Unity.Entities;
using Unity.Transforms;

namespace SmokGnu.SpriteSheetRenderer.Render.RenderGroup
{
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
}