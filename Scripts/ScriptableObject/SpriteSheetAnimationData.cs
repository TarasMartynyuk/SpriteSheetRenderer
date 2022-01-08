using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "SpriteSheetAnimationData", menuName = "SpriteSheetRenderer/SpriteSheetAnimationData", order = 0)]
public class SpriteSheetAnimationData : ScriptableObject
{
    public Sprite[] sprites;
    public Sprite SpriteSheet;
    
    public int startIndex;
    public bool playOnStart = true;
    public float duration;
    public float frameDuration => duration / sprites.Length;

    public string animationName;
    public string Name => animationName;

    public SpriteSheetAnimationComponent.RepetitionType repetition = SpriteSheetAnimationComponent.RepetitionType.Loop;

}


