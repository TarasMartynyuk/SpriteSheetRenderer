using SmokGnu.SpriteSheetRenderer.Animation.Components;
using SmokGnu.SpriteSheetRenderer.Render;
using SmokGnu.SpriteSheetRenderer.Utils.TMUtilsEcs;
using SmokGnu.SpriteSheetRenderer.Utils.TMUtilsEcs.DOTS;
using Unity.Entities;
using UnityEngine;

namespace SmokGnu.SpriteSheetRenderer
{
    public static class SpriteSheetRendererInit
    {
        public static void Init(Shader spriteSheetShader)
        {
            var eManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            eManager.CreateEntity("SpriteSheetAnimationSingleton", typeof(AnimationChangeCommandBufferElement));
            EcsSystemUtils.CreateSystemManaged<SpriteSheetRenderSystem, PresentationSystemGroup>().Init(spriteSheetShader);
        }
    }
}