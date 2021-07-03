using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Collections.LowLevel.Unsafe;


[InternalBufferCapacity(16)]
public struct MatrixBuffer : IBufferElementData
{
    public static implicit operator float4x4(MatrixBuffer e) { return e.matrix; }
    public static implicit operator MatrixBuffer(float4x4 e) { return new MatrixBuffer { matrix = e }; }
    public float4x4 matrix;

    public static DynamicBuffer<float4x4> GetMatrixBuffer(Entity entity)
    {
        var eManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        return eManager.GetBuffer<MatrixBuffer>(entity).Reinterpret<float4x4>();
    }

    public static int SizeOf() => UnsafeUtility.SizeOf<float4x4>();
}