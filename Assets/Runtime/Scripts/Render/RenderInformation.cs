﻿using Unity.Entities;
using UnityEngine;

namespace SmokGnu.SpriteSheetRenderer.Render
{
    public class RenderInformation
    {
        public uint[] args;
        public ComputeBuffer argsBuffer;
        public Entity renderGroup;
        public ComputeBuffer colorsBuffer;
        public ComputeBuffer indexBuffer;
        public Material material;
        public ComputeBuffer matrixBuffer;
        public int spriteCount;
        public bool updateUvs;
        public ComputeBuffer uvBuffer;

        public RenderInformation(Material material, int materialLength, Entity renderGroup)
        {
            this.material = material;
            spriteCount = materialLength;
            this.renderGroup = renderGroup;
            args = new uint[5] {0, 0, 0, 0, 0};
            argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
            //thoose args are always the same since we always use the same mesh
            args[0] = (uint) 6;
            args[2] = args[3] = (uint) 0;
            updateUvs = true;
        }

        public void DestroyBuffers()
        {
            if (matrixBuffer != null)
                matrixBuffer.Release();
            matrixBuffer = null;

            if (argsBuffer != null)
                argsBuffer.Release();
            argsBuffer = null;

            if (colorsBuffer != null)
                colorsBuffer.Release();
            colorsBuffer = null;

            if (uvBuffer != null)
                uvBuffer.Release();
            uvBuffer = null;

            if (indexBuffer != null)
                indexBuffer.Release();
            indexBuffer = null;
        }
    }
}