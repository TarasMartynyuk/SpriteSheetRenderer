using System;
using System.Collections.Generic;
using TMUtilsEcs.DOTS.Ecs;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public static class SpriteSheetRendererInit
{
    public static void Init(Shader spriteSheetShader)
    {
        var eManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        eManager.CreateEntity("SpriteSheetAnimationSingleton", typeof(AnimationChangeCommandBufferElement));
        EcsSystemUtils.CreateSystem<SpriteSheetRenderSystem, PresentationSystemGroup>().Init(spriteSheetShader);
    }
}