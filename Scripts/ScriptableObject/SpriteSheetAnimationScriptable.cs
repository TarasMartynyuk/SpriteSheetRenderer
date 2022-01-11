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
    public Entity definitionEntity { get; private set; }
    
    [SerializeField] SpriteSheetAnimationDefinitionComponent definition;

    public void Init(int indexInAnimator)
    {
        definition.frameDuration = definition.duration / sprites.Length;
        definition.IndexInAnimator = indexInAnimator;
        var eManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        definitionEntity = eManager.CreateEntity(SpriteSheetAnimationFactory.Instance.SpriteSheetAnimationDefinition.Archetype);
        eManager.SetComponentData(definitionEntity, definition);
    }


}


