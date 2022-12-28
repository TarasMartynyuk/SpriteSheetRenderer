using SmokGnu.SpriteSheetRenderer.Render.RenderGroup.Components;
using SmokGnu.SpriteSheetRenderer.Utils;
using Unity.Entities;
using UnityEngine;

namespace SmokGnu.SpriteSheetRenderer.ScriptableObject
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "SpriteSheetAnimationData", menuName = "SpriteSheetRenderer/SpriteSheetAnimationData", order = 0)]
    public class SpriteSheetAnimationScriptable : UnityEngine.ScriptableObject
    {
        public Sprite[] Sprites;
        public Sprite SpriteSheet;
        public Entity RenderGroup { get; private set; }

        public void Init(Entity renderGroup)
        {
            Debug.Assert(m_definition.Duration != 0, $"duration == 0, {name}");

            if (m_eventFrame.HasValue)
                m_definition.EventFrame = m_eventFrame.Value;
            m_definition.SpriteCount = Sprites.Length;
            m_definition.FrameDuration = m_definition.Duration / Sprites.Length;
            var eManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            eManager.SetComponentData(renderGroup, m_definition);
            RenderGroup = renderGroup;
        }
        // public Entity DefinitionEntity { get; private set; }

        [SerializeField] SpriteSheetAnimationDefinitionComponent m_definition;
        [SerializeField] SerializableNullable<int> m_eventFrame;
    }
}


