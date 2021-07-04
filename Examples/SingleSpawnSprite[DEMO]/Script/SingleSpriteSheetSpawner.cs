using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace SpriteSheetRendererExamples
{
    public class SingleSpriteSheetSpawner : MonoBehaviour, IConvertGameObjectToEntity
    {
        public Sprite[] sprites;
        public void Convert(Entity entity, EntityManager eManager, GameObjectConversionSystem conversionSystem)
        {
            // 1) Create Archetype
            //EntityArchetype archetype = eManager.CreateArchetype(
            //         typeof(Position2D),
            //         typeof(Rotation2D),
            //         typeof(Scale),
            //         //required params
            //         typeof(SpriteIndex),
            //         typeof(SpriteSheetAnimation),
            //         typeof(SpriteSheetMaterial),
            //         typeof(SpriteSheetColor),
            //         typeof(SpriteMatrix),
            //         typeof(BufferHook)
            //      );

            //// 2) Record and bake this spritesheet(only once)
            //SpriteSheetManager.RecordSpriteSheet(sprites, "emoji");

            //int maxSprites = SpriteSheetCache.GetLength("emoji");
            //var color = UnityEngine.Random.ColorHSV(.35f, .85f);

            //// 4) Instantiate the entity
            //Entity e = SpriteSheetManager.Instantiate(archetype, "emoji");

            //// 3) Populate components
            //eManager.AddComponentData(e, new Position2D { Value = float2.zero });
            //eManager.AddComponentData(e, new Scale { Value = 15 });
            //eManager.AddComponentData(e, new SpriteIndex { Value = UnityEngine.Random.Range(0, maxSprites) });
            //eManager.AddComponentData(e, new SpriteSheetAnimation { maxSprites = maxSprites, play = true, repetition = SpriteSheetAnimation.RepetitionType.Loop, samples = 10 });
            //eManager.AddComponentData(e, new SpriteSheetColor { color = new float4(color.r, color.g, color.b, color.a) });
        }
    }
}