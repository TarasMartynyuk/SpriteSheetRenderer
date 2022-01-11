
using System;
using Unity.Entities;
using UnityEngine;

public enum RepetitionType
{
    Invalid,
    Once,
    Loop,
    PingPong
}

[Serializable]
public struct SpriteSheetAnimationDefinitionComponent : IComponentData
{
    public int startIndex;
    public bool playOnStart;
    public float duration;
    public RepetitionType repetition;
    [HideInInspector]
    public float frameDuration;
    [HideInInspector]
    public int maxSprites;

    public int EventFrame;

    // in animator list
    // public int animationIndex;
}
