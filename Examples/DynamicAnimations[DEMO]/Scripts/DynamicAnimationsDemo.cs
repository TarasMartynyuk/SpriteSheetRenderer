using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace SpriteSheetRendererExamples
{
    public class DynamicAnimationsDemo : MonoBehaviour, IConvertGameObjectToEntity
    {
        public SpriteSheetAnimator animator;
        public static Entity character;
        public void Convert(Entity entity, EntityManager eManager, GameObjectConversionSystem conversionSystem)
        {

            //eManager.SetNameInd(entity, "CONVERt ENTITY");

            // 1) Create Archetype
            EntityArchetype archetype = eManager.CreateArchetype(
                     typeof(LocalToWorld),
                     typeof(Translation),
                     typeof(Rotation),
                     typeof(NonUniformScale),
                     //required params
                     typeof(SpriteIndex),
                     typeof(SpriteSheetAnimation),
                     typeof(SpriteSheetMaterial),
                     typeof(SpriteSheetColor),
                     typeof(BufferHook)
            );
            SpriteSheetManager.RecordAnimator(animator);


            // 4) Instantiate the entity
            character = SpriteSheetManager.Instantiate(archetype, animator);
            //eManager.SetName(character, "DynamicAnimationsDemo");
            // 3) Populate components
            var color = Color.white;
            eManager.AddComponentData(character, new Translation { Value = float3.zero });
            eManager.AddComponentData(character, new NonUniformScale { Value = new float3(-3, 7, 0) });
            eManager.AddComponentData(character, new SpriteSheetColor { color = new float4(color.r, color.g, color.b, color.a) });

            SpriteMovement.Sprite = character;
        }
    }
}