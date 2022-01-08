using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "SpriteSheetAnimationData", menuName = "SpriteSheetRenderer/SpriteSheetAnimationData", order = 0)]
public class SpriteSheetAnimationDataScriptable : ScriptableObject
{
    public Sprite[] sprites;
    public Sprite SpriteSheet;
    
    public int startIndex;
    public bool playOnStart = true;
    public float duration;
    public float frameDuration => duration / sprites.Length;

    public string fullName { get; private set;  }
    public string Name => animationName;
    [SerializeField] string animationName;

    public SpriteSheetAnimationComponent.RepetitionType repetition = SpriteSheetAnimationComponent.RepetitionType.Loop;

    public void Init(SpriteSheetAnimator parentAnimator)
    {
        fullName = $"{parentAnimator.animationsPrefix}_{animationName}";
    }
    
    // editor-only - use this in custom editor for this scriptable
    public void RetrieveSheetSprites()
    {
        string spriteSheet = AssetDatabase.GetAssetPath( SpriteSheet.texture );
        sprites = AssetDatabase.LoadAllAssetsAtPath( spriteSheet )
            .OfType<Sprite>().ToArray();
    }
}


