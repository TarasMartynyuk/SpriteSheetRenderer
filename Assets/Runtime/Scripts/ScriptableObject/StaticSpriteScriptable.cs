using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
[CreateAssetMenu(fileName = "StaticSprite", menuName = "SpriteSheetRenderer/StaticSprite", order = 0)]
public class StaticSpriteScriptable : ScriptableObject
{
    public Sprite Sprite;
    public Entity RenderGroup { get; private set; }

    public void Init(Entity renderGroup)
    {
        RenderGroup = renderGroup;
    }
}


