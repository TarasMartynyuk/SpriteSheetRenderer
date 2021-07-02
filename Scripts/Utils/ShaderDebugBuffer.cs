﻿using UnityEngine;
using Unity.Collections.LowLevel.Unsafe;
using System;

public class ShaderDebugBuffer<T> : IDisposable
    where T : struct
{
    public ComputeBuffer ComputeBuffer {get; private set;}
    public T[] m_debugBufferRetrieved {get; private set;}

    public Material Material { get; set; }

    public ShaderDebugBuffer(int size)
    {
        int stride = UnsafeUtility.SizeOf<T>();

        Debug.Log($"Debug Buffer: stride is {stride}");
        ComputeBuffer = new ComputeBuffer(size, stride, ComputeBufferType.Default);
        m_debugBufferRetrieved = new T[size];
    }

    public T[] GetBufferData()
    {
        Graphics.ClearRandomWriteTargets();
        Material.SetBuffer("_DebugBuffer", ComputeBuffer);
        Graphics.SetRandomWriteTarget(1, ComputeBuffer, false);
        ComputeBuffer.GetData(m_debugBufferRetrieved);
        return m_debugBufferRetrieved;
    }

    public void Dispose() => ComputeBuffer.Release();
}
