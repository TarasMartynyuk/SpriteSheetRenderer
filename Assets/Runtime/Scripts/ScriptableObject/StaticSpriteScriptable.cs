using Unity.Entities;
using UnityEngine;

namespace SmokGnu.SpriteSheetRenderer.ScriptableObject
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "StaticSprite", menuName = "SpriteSheetRenderer/StaticSprite", order = 0)]
    public class StaticSpriteScriptable : UnityEngine.ScriptableObject
    {
        public Sprite Sprite;
        public Entity RenderGroup { get; private set; }

        public void Init(Entity renderGroup)
        {
            RenderGroup = renderGroup;
        }
    }
}


