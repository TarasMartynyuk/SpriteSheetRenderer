using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
[CreateAssetMenu(fileName = "SpriteSheetAnimationData", menuName = "SpriteSheetRenderer/SpriteSheetAnimationData", order = 0)]
public class SpriteSheetAnimationScriptable : ScriptableObject
{
    public Sprite[] Sprites;
    public Sprite SpriteSheet;
    public string AnimationName;
    public Entity DefinitionEntity { get; private set; }
    
    [SerializeField] SpriteSheetAnimationDefinitionComponent m_definition;
    [SerializeField] SerializableNullable<int> m_eventFrame;

    public void Init(int indexInAnimator)
    {
        Debug.Assert(m_definition.Duration != 0, $"duration == 0, {AnimationName}");

        if (m_eventFrame.HasValue)
            m_definition.EventFrame = m_eventFrame.Value;
        m_definition.SpriteCount = Sprites.Length;
        m_definition.FrameDuration = m_definition.Duration / Sprites.Length;
        m_definition.IndexInAnimator = indexInAnimator;
        var eManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        DefinitionEntity = eManager.CreateEntity(SpriteSheetAnimationFactory.Instance.SpriteSheetAnimationDefinition.Archetype);
        eManager.SetComponentData(DefinitionEntity, m_definition);
    }


}


