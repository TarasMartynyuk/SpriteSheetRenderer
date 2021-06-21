﻿using Unity.Entities;
using Unity.Transforms;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;

public class SpriteSheetPositionSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref SpriteMatrix renderData, in Position2D translation) =>
        {
            renderData.matrix.x = translation.Value.x;
            renderData.matrix.y = translation.Value.y;
        })
        .WithChangeFilter<Position2D>()
        .Schedule();
    }
}
