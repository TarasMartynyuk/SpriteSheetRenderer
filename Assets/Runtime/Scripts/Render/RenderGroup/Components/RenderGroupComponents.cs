using SmokGnu.SpriteSheetRenderer.Utils.TMUtilsEcs.DOTS;
using System;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace SmokGnu.SpriteSheetRenderer.Render.RenderGroup.Components
{
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

    [InternalBufferCapacity(8)]
    public struct SpriteColorBufferElement : IBufferElementData
    {
        public static implicit operator float4(SpriteColorBufferElement e) { return e.color; }
        public static implicit operator SpriteColorBufferElement(float4 e) { return new SpriteColorBufferElement {color = e}; }
        public float4 color;

        public static DynamicBuffer<float4> GetColors(Entity e) => DotsCollectionUtils.GetReinterprettedBuffer<float4, SpriteColorBufferElement>(e);
    }

    [InternalBufferCapacity(sizeof(int))]
    public struct SpriteIndexBuffer : IBufferElementData
    {
        public static implicit operator int(SpriteIndexBuffer e)
        {
            return e.index;
        }

        public static implicit operator SpriteIndexBuffer(int e)
        {
            return new SpriteIndexBuffer {index = e};
        }

        public int index;

        public static DynamicBuffer<int> GetSpriteIndices(Entity e) => DotsCollectionUtils.GetReinterprettedBuffer<int, SpriteIndexBuffer>(e);
    }

// This describes the number of buffer elements that should be reserved
// in chunk data for each instance of a buffer. In this case, 8 integers
// will be reserved (32 bytes) along with the size of the buffer header
// (currently 16 bytes on 64-bit targets)
    [InternalBufferCapacity(8)]
    public struct UvBuffer : IBufferElementData
    {
        public float4 Value;
        public static DynamicBuffer<float4> GetUV(Entity e) => DotsCollectionUtils.GetReinterprettedBuffer<float4, UvBuffer>(e);
    }

    public struct RenderedEntityBufferElement : IBufferElementData
    {
        public Entity Value;

        public static DynamicBuffer<Entity> GetRenderedEntities(Entity e) =>
            DotsCollectionUtils.GetReinterprettedBuffer<Entity, RenderedEntityBufferElement>(e);
    }

    public enum RepetitionType
    {
        Invalid,
        Once,
        Loop,
        PingPong
    }

    [Serializable]
    public struct SpriteSheetAnimationDefinitionComponent : IComponentData
    {
        public float Duration;
        public RepetitionType Repetition;

        [HideInInspector]
        public float FrameDuration;
        [HideInInspector]
        public int SpriteCount;
        [HideInInspector]
        public int? EventFrame;
    }
}