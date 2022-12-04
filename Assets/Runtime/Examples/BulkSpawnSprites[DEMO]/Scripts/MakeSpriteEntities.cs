// using System;
// using System.Collections.Generic;
// using Unity.Collections;
// using Unity.Entities;
// using Unity.Mathematics;
// using Unity.Transforms;
// using UnityEngine;
// using Random = Unity.Mathematics.Random;
// namespace SpriteSheetRendererExamples
// {
//
//     public class MakeSpriteEntities : MonoBehaviour, IConvertGameObjectToEntity
//     {
//         public float2 spawnArea = new float2(100, 100);
//         public int spriteCount = 5000;
//         public Sprite[] sprites;
//
//         public void Convert(Entity entity, EntityManager eManager, GameObjectConversionSystem conversionSystem)
//         {
//             throw new NotImplementedException();
//             
//         }
//
//         private void OnDrawGizmosSelected()
//         {
//             var r = GetSpawnArea();
//             Gizmos.color = new Color(0, .35f, .45f, .24f);
//             Gizmos.DrawCube(r.center, r.size);
//         }
//
//         Rect GetSpawnArea()
//         {
//             Rect r = new Rect(0, 0, spawnArea.x, spawnArea.y);
//             r.center = transform.position;
//             return r;
//         }
//     }
// }