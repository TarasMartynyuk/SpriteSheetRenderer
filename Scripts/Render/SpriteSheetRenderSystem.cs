using System.Text;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class SpriteSheetRenderSystem : SystemBase
{
    protected override void OnCreate()
    {
        mesh = MeshExtension.Quad();
    }

    protected override void OnDestroy()
    {
        SpriteSheetManager.Instance.CleanBuffers();
        m_debugBuffer.Dispose();
    }

    protected override void OnUpdate()
    {

        for (int i = 0; i < SpriteSheetManager.Instance.RenderInformation.Count; i++)
        {
            var renderInformation = SpriteSheetManager.Instance.RenderInformation[i];
            
            // if (i == 1)
            // {
            //     m_debugBuffer.Material = SpriteSheetManager.Instance.RenderInformation[i].material;
            //     var debugData = m_debugBuffer.GetBufferData();
            // }
            
            

            if (UpdateBuffers(i) > 0)
            {
                // todo: allow setting dimensions (to the dimensions of map, e.g.), or to the frustrum bounds dynamically
                var dimensions = new float3(1000);
                var bounds = new Bounds(dimensions / 2, dimensions);

                
                
                Graphics.DrawMeshInstancedIndirect(mesh, 0, SpriteSheetManager.Instance.RenderInformation[i].material, bounds, SpriteSheetManager.Instance.RenderInformation[i].argsBuffer);
            }


            //this is w.i.p to clean the old buffers
            DynamicBuffer<SpriteIndexBuffer> indexBuffer = EntityManager.GetBuffer<SpriteIndexBuffer>(SpriteSheetManager.Instance.RenderInformation[i].renderGroup);
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
                EntityManager.GetBuffer<SpriteIndexBuffer>(SpriteSheetManager.Instance.RenderInformation[i].renderGroup).RemoveRange(size + 1 - toRemove, toRemove);
                EntityManager.GetBuffer<MatrixBuffer>(SpriteSheetManager.Instance.RenderInformation[i].renderGroup).RemoveRange(size + 1 - toRemove, toRemove);
                EntityManager.GetBuffer<SpriteColorBufferElement>(SpriteSheetManager.Instance.RenderInformation[i].renderGroup).RemoveRange(size + 1 - toRemove, toRemove);
            }
        }
        
        // var sb = new StringBuilder();
        // Entities.ForEach((Entity e, in SpriteSheetRenderGroupHookComponent hook) =>
        //     {
        //         sb.Append($"{e.Stringify()}: {hook}");
        //     })
        //     .WithoutBurst()
        //     .Run();
        //
        // Debug.Log($"hooks : {sb}");
    }

    ShaderDebugBuffer<Matrix4x4> m_debugBuffer = new ShaderDebugBuffer<Matrix4x4>(3);
    private Mesh mesh;

    //we should only update the index of the changed datas for index buffer,matrixbuffer and color buffer inside a burst job to avoid overhead
    int UpdateBuffers(int renderIndex)
    {
        SpriteSheetManager.Instance.ReleaseBuffer(renderIndex);

        RenderInformation renderInformation = SpriteSheetManager.Instance.RenderInformation[renderIndex];
        int instanceCount = EntityManager.GetBuffer<SpriteIndexBuffer>(renderInformation.renderGroup).Length;
        if (instanceCount > 0)
        {

            int stride = instanceCount >= 16 ? 16 : 16 * SpriteSheetCache.Instance.GetLenght(renderInformation.material);


            if (renderInformation.updateUvs)
            {
                SpriteSheetManager.Instance.ReleaseUvBuffer(renderIndex);
                renderInformation.uvBuffer = new ComputeBuffer(instanceCount, stride);
                renderInformation.uvBuffer.SetData(EntityManager.GetBuffer<UvBuffer>(renderInformation.renderGroup).Reinterpret<float4>().AsNativeArray());
                renderInformation.material.SetBuffer("uvBuffer", renderInformation.uvBuffer);
                renderInformation.updateUvs = false;
            }

            renderInformation.indexBuffer = new ComputeBuffer(instanceCount, sizeof(int));
            renderInformation.indexBuffer.SetData(EntityManager.GetBuffer<SpriteIndexBuffer>(renderInformation.renderGroup).Reinterpret<int>().AsNativeArray());
            renderInformation.material.SetBuffer("indexBuffer", renderInformation.indexBuffer);

            renderInformation.matrixBuffer = new ComputeBuffer(instanceCount, MatrixBuffer.SizeOf());
            renderInformation.matrixBuffer.SetData(MatrixBuffer.GetMatrixBuffer(renderInformation.renderGroup).AsNativeArray());
            renderInformation.material.SetBuffer("matrixBuffer", renderInformation.matrixBuffer);

            renderInformation.args[1] = (uint) instanceCount;
            renderInformation.argsBuffer.SetData(renderInformation.args);

            renderInformation.colorsBuffer = new ComputeBuffer(instanceCount, 16);
            renderInformation.colorsBuffer.SetData(EntityManager.GetBuffer<SpriteColorBufferElement>(renderInformation.renderGroup).Reinterpret<float4>().AsNativeArray());
            renderInformation.material.SetBuffer("colorsBuffer", renderInformation.colorsBuffer);

            // if (renderInformation.renderGroup.Stringify().Contains("Legion"))
            // {
            //     DebugExtensions.LogVar(new
            //     {
            //         anim = renderInformation.renderGroup.Stringify(),
            //         idxs = renderInformation.indexBuffer.GetData<int>().Stringify(),
            //         matrixB = renderInformation.matrixBuffer.GetData<float4x4>().Stringify(),
            //         uvs = UvBuffer.GetUV(renderInformation.renderGroup).Stringify(),
            //         // args = renderInformation.argsBuffer.GetData<uint>(),
            //         colors = renderInformation.colorsBuffer.GetData<float4>().Stringify(),
            //     });
            //     
            // }
            
        }
        return instanceCount;
    }

    
}

public static class E
{
    public static T[] GetData<T>(this ComputeBuffer computeBuffer)
    {
        var arr = new T[computeBuffer.count];
        computeBuffer.GetData(arr);
        return arr;
    }
}