using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace SpriteSheetRendererExamples
{
    public class SingleEntityDestroy : MonoBehaviour, IConvertGameObjectToEntity
    {
        public Sprite[] sprites;

        public void Convert(Entity entity, EntityManager eManager, GameObjectConversionSystem conversionSystem)
        {
            throw new NotImplementedException();
            //Record and bake this spritesheets(only once)
            archetype = eManager.CreateArchetype(
                typeof(LocalToWorld),
                //typeof(Rotation2D),
                typeof(Scale),
                typeof(LifeTime),
                //required params
                typeof(SpriteIndex),
                typeof(SpriteSheetAnimationComponent),
                typeof(Material),
                typeof(SpriteSheetColor),
                //typeof(SpriteMatrix),
                typeof(SpriteSheetRenderGroupHookComponent)
            );
            // SpriteSheetManager.RecordSpriteSheet(sprites, "emoji");
        }

        EntityArchetype archetype;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                int maxSprites = SpriteSheetCache.Instance.GetLength("emoji");
                var color = UnityEngine.Random.ColorHSV(.35f, .85f);


                throw new NotImplementedException();

                // 3) Populate components
                List<IComponentData> components = new List<IComponentData>
                {
                    //new Position2D { Value = UnityEngine.Random.insideUnitCircle * 7 },
                    //new Position2D { Value = UnityEngine.Random.insideUnitCircle * 7 },
                    new Scale {Value = UnityEngine.Random.Range(0, 3f)},
                    new SpriteIndex {Value = UnityEngine.Random.Range(0, maxSprites)},
                    // new SpriteSheetAnimationComponent { maxSprites = maxSprites, isPlaying = true, repetition = SpriteSheetAnimationComponent.RepetitionType.Loop, samples = 10 },
                    new SpriteSheetColor {color = new float4(color.r, color.g, color.b, color.a)},
                    new LifeTime {Value = UnityEngine.Random.Range(5, 15)}
                };

                // SpriteSheetManager.Instantiate(archetype, "emoji");
            }
        }
    }
}