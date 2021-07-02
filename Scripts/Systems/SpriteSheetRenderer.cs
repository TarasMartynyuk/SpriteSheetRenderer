using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class SpriteSheetRenderer : SystemBase
{
    private Mesh mesh;
    ShaderDebugBuffer<Vector4> m_debugBuffer = new ShaderDebugBuffer<Vector4>(1);

    protected override void OnCreate()
    {
        mesh = MeshExtension.Quad();
    }

    protected override void OnDestroy()
    {
        SpriteSheetManager.CleanBuffers();
        m_debugBuffer.Dispose();
    }
    protected override void OnUpdate()
    {

        for (int i = 0; i < SpriteSheetManager.renderInformation.Count; i++)
        {
            if (i == 1)
            {
                m_debugBuffer.Material = SpriteSheetManager.renderInformation[i].material;
                var debugData = m_debugBuffer.GetBufferData();
                Debug.Log($"after: {debugData[0]:F3}");
            }

            if (UpdateBuffers(i) > 0)
                Graphics.DrawMeshInstancedIndirect(mesh, 0, SpriteSheetManager.renderInformation[i].material, new Bounds(Vector2.zero, Vector3.one), SpriteSheetManager.renderInformation[i].argsBuffer);

            //this is w.i.p to clean the old buffers
            DynamicBuffer<SpriteIndexBuffer> indexBuffer = EntityManager.GetBuffer<SpriteIndexBuffer>(SpriteSheetManager.renderInformation[i].bufferEntity);
            int size = indexBuffer.Length - 1;
            int toRemove = 0;
            for (int j = size; j >= 0; j--)
            {
                if (indexBuffer[j].index == -1)
                {
                    toRemove++;
                }
                else
                {
                    break;
                }
            }
            if (toRemove > 0)
            {
                EntityManager.GetBuffer<SpriteIndexBuffer>(SpriteSheetManager.renderInformation[i].bufferEntity).RemoveRange(size + 1 - toRemove, toRemove);
                EntityManager.GetBuffer<MatrixBuffer>(SpriteSheetManager.renderInformation[i].bufferEntity).RemoveRange(size + 1 - toRemove, toRemove);
                EntityManager.GetBuffer<SpriteColorBuffer>(SpriteSheetManager.renderInformation[i].bufferEntity).RemoveRange(size + 1 - toRemove, toRemove);
            }
        }
    }
    //we should only update the index of the changed datas for index buffer,matrixbuffer and color buffer inside a burst job to avoid overhead
    int UpdateBuffers(int renderIndex)
    {
        SpriteSheetManager.ReleaseBuffer(renderIndex);

        RenderInformation renderInformation = SpriteSheetManager.renderInformation[renderIndex];
        int instanceCount = EntityManager.GetBuffer<SpriteIndexBuffer>(renderInformation.bufferEntity).Length;
        if (instanceCount > 0)
        {
            int stride = instanceCount >= 16 ? 16 : 16 * SpriteSheetCache.GetLenght(renderInformation.material);
            if (renderInformation.updateUvs)
            {
                SpriteSheetManager.ReleaseUvBuffer(renderIndex);
                renderInformation.uvBuffer = new ComputeBuffer(instanceCount, stride);
                renderInformation.uvBuffer.SetData(EntityManager.GetBuffer<UvBuffer>(renderInformation.bufferEntity).Reinterpret<float4>().AsNativeArray());
                renderInformation.material.SetBuffer("uvBuffer", renderInformation.uvBuffer);
                renderInformation.updateUvs = false;
            }

            renderInformation.indexBuffer = new ComputeBuffer(instanceCount, sizeof(int));
            renderInformation.indexBuffer.SetData(EntityManager.GetBuffer<SpriteIndexBuffer>(renderInformation.bufferEntity).Reinterpret<int>().AsNativeArray());
            renderInformation.material.SetBuffer("indexBuffer", renderInformation.indexBuffer);

            //renderInformation.matrixBuffer = new ComputeBuffer(instanceCount, 16);
            renderInformation.matrixBuffer = new ComputeBuffer(instanceCount, 4 * 16); //UnsafeUtility.SizeOf<float4x4>());
            renderInformation.matrixBuffer.SetData(MatrixBuffer.GetMatrixBuffer(renderInformation.bufferEntity).AsNativeArray());
            renderInformation.material.SetBuffer("matrixBuffer", renderInformation.matrixBuffer);

            renderInformation.args[1] = (uint) instanceCount;
            renderInformation.argsBuffer.SetData(renderInformation.args);

            renderInformation.colorsBuffer = new ComputeBuffer(instanceCount, 16);
            renderInformation.colorsBuffer.SetData(EntityManager.GetBuffer<SpriteColorBuffer>(renderInformation.bufferEntity).Reinterpret<float4>().AsNativeArray());
            renderInformation.material.SetBuffer("colorsBuffer", renderInformation.colorsBuffer);
        }
        return instanceCount;
    }

}