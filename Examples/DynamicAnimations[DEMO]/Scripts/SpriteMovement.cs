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

        [SerializeField] bool m_moving;
        [SerializeField] bool m_rotating;

        const float m_speed = 5.0f;
        const float m_360RotationTime = 4.0f;
        float3 m_startPosition;

        private void Start()
        {
            var eManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            m_startPosition = eManager.GetComponentData<LocalToWorld>(Sprite).Position;
        }

        void Update()
        {
            var eManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            if (m_rotating)
            {
                float timePassed = (Time.realtimeSinceStartup % m_360RotationTime);
                float rotationPercent = timePassed / m_360RotationTime;
                var angle = 360.0f * rotationPercent;
                var rot = quaternion.RotateZ(angle);

                DebugExtensions.LogVar(new
                {
                    Time.frameCount,
                    Time.realtimeSinceStartup,
                    timePassed,
                    rotationPercent,
                    angle,
                    rot = ((Quaternion) rot).eulerAngles
                });

                eManager.SetComponentData(Sprite, new Rotation { Value = rot });
            }

            if (!m_moving)
                return;

            float percent = (Time.realtimeSinceStartup % 1.0f);
            float2 offset = new float2(1) * ((percent * m_speed) - m_speed / 2);
            eManager.SetComponentData(Sprite, new Translation { Value = m_startPosition + new float3(offset, 0) });

        }
    }
}

public static class DebugExtensions
{
    [Conditional("UNITY_EDITOR")]
    public static void LogIf(string str, bool condition)
    {
        if (condition)
        {
            Debug.Log(str);
        }
    }

    public static void LogVar<T>(T myInput) where T : class
    {
        string valueStr = myInput.ToString();
        valueStr = valueStr.Substring(valueStr.IndexOf("= ") + 2);
        valueStr = valueStr.Substring(0, valueStr.Length - 2);
        Debug.Log($"{typeof(T).GetProperties()[0].Name}: {valueStr}");
    }
}