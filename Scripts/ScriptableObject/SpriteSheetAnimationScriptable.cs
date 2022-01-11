using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using UnityEditor;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "SpriteSheetAnimationData", menuName = "SpriteSheetRenderer/SpriteSheetAnimationData", order = 0)]
public class SpriteSheetAnimationScriptable : ScriptableObject
{
    public Sprite[] sprites;
    public Sprite SpriteSheet;
    public string animationName;
    public SpriteSheetAnimationDefinitionComponent definition;
    
    public Entity definitionEntity { get; private set; }

    public void Init()
    {
        definition.frameDuration = definition.duration / sprites.Length;
        var eManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        definitionEntity = eManager.CreateEntity(SpriteSheetAnimationFactory.Instance.SpriteSheetAnimationDefinition.Archetype);
        eManager.SetComponentData(definitionEntity, definition);
    }


}


