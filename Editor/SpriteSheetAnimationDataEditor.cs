using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpriteSheetAnimationData))]
public class SpriteSheetAnimationDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        // var script = (SpriteSheetAnimationData)target;
 
        // if(GUILayout.Button("RetrieveSheetSprites", GUILayout.Height(40)))
        // {
            // script.RetrieveSheetSprites();
        // }

        if (GUILayout.Button("RetrieveSheetSprites"))
        {
            
        }
         
    }
}