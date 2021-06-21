﻿using Unity.Entities;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;

public class SpriteSheetRotationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref SpriteMatrix renderData, in Rotation2D rotation) =>
        {
            renderData.matrix.z = rotation.angle;
        }).Schedule();
    }
}
