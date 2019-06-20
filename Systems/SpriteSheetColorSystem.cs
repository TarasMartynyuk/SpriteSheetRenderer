﻿using Unity.Entities;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;

[UpdateAfter(typeof(SpriteSheetAnimationSystem))]
public class SpriteSheetColorSystem : JobComponentSystem {
  [BurstCompile]
  struct SpriteSheetColorJob : IJobForEach<SpriteSheetColor, RenderData> {
    public void Execute([ReadOnly] ref SpriteSheetColor color, ref RenderData renderData) {
      renderData.color = color.value;
    }
  }

  protected override JobHandle OnUpdate(JobHandle inputDeps) {
    var job = new SpriteSheetColorJob() { };
    return job.Schedule(this, inputDeps);
  }
}
