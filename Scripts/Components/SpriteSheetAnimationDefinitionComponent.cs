
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
    public float duration;
    public RepetitionType repetition;
    public int EventFrame;

    [HideInInspector]
    public float frameDuration;
    [HideInInspector]
    public int maxSprites;
    [HideInInspector]
    public int IndexInAnimator;
}
