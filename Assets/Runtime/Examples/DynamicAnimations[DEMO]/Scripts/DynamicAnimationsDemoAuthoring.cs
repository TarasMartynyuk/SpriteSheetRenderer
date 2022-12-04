using System.Collections;
using System.Collections.Generic;
using TMUtilsEcs.DOTS.Factories;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace SpriteSheetRendererExamples
{
    
    //MY_CMP_ERROR
    public class DynamicAnimationsDemoAuthoring : MonoBehaviour
    {
        public static Entity character;

        public SpriteSheetAnimator animator;

        [field: SerializeField] public Shader Shader { get; }
    }
}