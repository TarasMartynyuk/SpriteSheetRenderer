
using System;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

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
    public float Duration;
    public RepetitionType Repetition;

    [HideInInspector]
    public float FrameDuration;
    [HideInInspector]
    public int SpriteCount;
    [HideInInspector]
    public int? EventFrame;
}
