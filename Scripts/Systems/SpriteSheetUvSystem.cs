using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public class SpriteSheetUvJobSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((in SpriteSheetRenderGroupHookComponent hook, in SpriteIndex data) =>
            {
                var buffer = GetBuffer<SpriteIndexBuffer>(hook.SpritesheetRenderGroup);
                buffer[hook.IndexInRenderGroup] = data.Value;
            })
            .WithChangeFilter<SpriteIndex>()
            .Schedule();
    }
}
