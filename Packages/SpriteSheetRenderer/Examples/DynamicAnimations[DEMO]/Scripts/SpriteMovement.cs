using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class SpriteMovement : MonoBehaviour
{
    public static Entity Sprite;

    const float m_speed = 5.0f;
    const float m_360RotationTime = 4.0f;
    float2 m_startPosition;

    private void Start()
    {
        var eManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        m_startPosition = eManager.GetComponentData<Position2D>(Sprite).Value;
    }

    void Update()
    {
        var eManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        //float percent = (Time.realtimeSinceStartup % 1.0f); 
        //float2 offset = new float2(1) * ((percent * m_speed) - m_speed / 2);
        //eManager.SetComponentData(Sprite, new Position2D { Value = m_startPosition + offset });

        float rotationPercent = (Time.realtimeSinceStartup % m_360RotationTime); 
        eManager.SetComponentData(Sprite, new Rotation2D { angle = 360.0f * rotationPercent});
    }
}
