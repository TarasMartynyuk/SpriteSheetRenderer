using SmokGnu.SpriteSheetRenderer.Utils.TMUtilsEcs;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace SpriteSheetRendererExamples
{
    public class SpriteMovement : MonoBehaviour
    {
        public static Entity Sprite;
        const float m_speed = 3.0f;
        const float m_360RotationTime = 500;

        [SerializeField] bool m_moving;
        [SerializeField] bool m_rotating;
        float3 m_startPosition;

        private void Start()
        {
            var eManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            m_startPosition = eManager.GetComponentData<LocalToWorld>(Sprite).Position;
        }

        void Update()
        {
            // MY_CMP_ERROR
            
            var eManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            if (m_rotating)
            {
                var timePassed = (Time.realtimeSinceStartup % m_360RotationTime);
                var rotationPercent = timePassed / m_360RotationTime;
                var angle = 360.0f * rotationPercent;
                var rot = quaternion.RotateZ(angle);

                // eManager.SetComponentData(Sprite, new Rotation { Value = rot });
            }

            if (!m_moving)
                return;

            var percent = (Time.realtimeSinceStartup % 1.0f);
            var offset = new float2(1) * ((percent * m_speed) - m_speed / 2);
            var pos = m_startPosition + new float3(offset, 0);

            using var transform = new ComponentReference<LocalTransform>(Sprite);
            transform.Value().Position = pos;
        }
    }
}
