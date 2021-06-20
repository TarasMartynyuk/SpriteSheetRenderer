using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class DynamicAnimationsDemo : MonoBehaviour, IConvertGameObjectToEntity
{
    public SpriteSheetAnimator animator;
    public static Entity character;
    public void Convert(Entity entity, EntityManager eManager, GameObjectConversionSystem conversionSystem)
    {
        // 1) Create Archetype
        EntityArchetype archetype = eManager.CreateArchetype(
                 typeof(Position2D),
                 typeof(Rotation2D),
                 typeof(Scale),
                 //required params
                 typeof(SpriteIndex),
                 typeof(SpriteSheetAnimation),
                 typeof(SpriteSheetMaterial),
                 typeof(SpriteSheetColor),
                 typeof(SpriteMatrix),
                 typeof(BufferHook)
        );
        SpriteSheetManager.RecordAnimator(animator);


        // 4) Instantiate the entity
        character = SpriteSheetManager.Instantiate(archetype, animator);
        // 3) Populate components
        var color = Color.white;
        eManager.AddComponentData(character, new Position2D { Value = float2.zero });
        eManager.AddComponentData(character, new Scale { Value = 5 });
        eManager.AddComponentData(character, new SpriteSheetColor { color = new float4(color.r, color.g, color.b, color.a) });

        SpriteMovement.Sprite = character;
    }
}
