using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpriteSheetRendererExamples
{
    public class GUIHelper : MonoBehaviour
    {
        public void ChangeAnimation(string animationName)
        {
            SpriteSheetManager.SetAnimation(DynamicAnimationsDemo.character, animationName);
        }
    }
}