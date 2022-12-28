using SmokGnu.SpriteSheetRenderer.Animation.Components;
using SmokGnu.SpriteSheetRenderer.Render.RenderGroup.Components;
using Unity.Collections;
using Unity.Entities;

namespace SmokGnu.SpriteSheetRenderer.Render.RenderGroup
{
    public partial class ColorBufferSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity e, in SpriteSheetRenderGroupHookComponent hook, in SpriteSheetColor data) =>
                {
                    var buffer = GetBuffer<SpriteColorBufferElement>(hook.SpritesheetRenderGroup);
                    buffer[hook.IndexInRenderGroup] = data.Value;
                })
                //.WithChangeFilter<SpriteSheetColor>()
                .WithoutBurst()
                .Run();
            // .Schedule();
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
}