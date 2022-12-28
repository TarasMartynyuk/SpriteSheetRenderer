using SmokGnu.SpriteSheetRenderer.Animation.Components;
using SmokGnu.SpriteSheetRenderer.Render.RenderGroup.Components;
using Unity.Entities;

namespace SmokGnu.SpriteSheetRenderer.Render.RenderGroup
{
    public partial class SpriteSheetUvJobSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((in SpriteSheetRenderGroupHookComponent hook, in SpriteIndex data) =>
                {
                    var buffer = GetBuffer<SpriteIndexBuffer>(hook.SpritesheetRenderGroup);
                    buffer[hook.IndexInRenderGroup] = data.Value;
                })
                // .WithChangeFilter<SpriteIhndex>()
                // .Schedule();
                .WithoutBurst()
                .Run();
        }
    }
}
