using System.Collections.Generic;
using System.Text;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[DisableAutoCreation]
public class SpriteSheetRenderSystem : SystemBase
{
    List<RenderInformation> RenderInformation { get; } = new();
    SpriteSheetCache m_spriteSheetCache = new();
    Mesh mesh;
    ShaderDebugBuffer<Matrix4x4> m_debugBuffer = new(3);

    public void Init(Shader spriteSheetShader)
    {
        m_spriteSheetCache.Init(spriteSheetShader);
    }
    
    public void RecordAnimator(SpriteSheetAnimator animator)
    {
        foreach (var animation in animator.animations)
        {
            var atlasData = m_spriteSheetCache.BakeSprites(animation.Sprites, animation.AnimationName);
            var newRenderGroup = RenderGroup.CreateRenderGroup(atlasData.Value, animation.AnimationName);
            animation.Init(newRenderGroup);

            RenderInformation.Add(new RenderInformation(atlasData.Key, m_spriteSheetCache.GetLength(atlasData.Key), newRenderGroup));
        }
    }

    public void RecordStaticSprite(StaticSpriteScriptable sprite)
    {
        var atlasData = m_spriteSheetCache.BakeSprite(sprite.Sprite, sprite.name);
        var renderGroup = RenderGroup.CreateRenderGroup(atlasData.Value, sprite.name);
        sprite.Init(renderGroup);
        RenderInformation.Add(new RenderInformation(atlasData.Key, m_spriteSheetCache.GetLength(atlasData.Key),  renderGroup));
    }

    protected override void OnCreate()
    {
        mesh = MeshExtension.Quad();
    }

    protected override void OnDestroy()
    {
        foreach (var r in RenderInformation)
            r.DestroyBuffers();

        RenderInformation.Clear();
        m_debugBuffer.Dispose();
    }

    protected override void OnUpdate()
    {

        for (int i = 0; i < RenderInformation.Count; i++)
        {
            var renderInformation = RenderInformation[i];
            
            if (UpdateBuffers(i) > 0)
            {
                // todo: allow setting dimensions (to the dimensions of map, e.g.), or to the frustrum bounds dynamically
                var dimensions = new float3(1000);
                var bounds = new Bounds(dimensions / 2, dimensions);

                
                
                Graphics.DrawMeshInstancedIndirect(mesh, 0, RenderInformation[i].material, bounds, RenderInformation[i].argsBuffer);
            }

            //this is w.i.p to clean the old buffers
            DynamicBuffer<SpriteIndexBuffer> indexBuffer = EntityManager.GetBuffer<SpriteIndexBuffer>(RenderInformation[i].renderGroup);
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
                EntityManager.GetBuffer<SpriteIndexBuffer>(RenderInformation[i].renderGroup).RemoveRange(size + 1 - toRemove, toRemove);
                EntityManager.GetBuffer<MatrixBuffer>(RenderInformation[i].renderGroup).RemoveRange(size + 1 - toRemove, toRemove);
                EntityManager.GetBuffer<SpriteColorBufferElement>(RenderInformation[i].renderGroup).RemoveRange(size + 1 - toRemove, toRemove);
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

    //we should only update the index of the changed datas for index buffer,matrixbuffer and color buffer inside a burst job to avoid overhead
    int UpdateBuffers(int renderIndex)
    {
        ReleaseBuffer(renderIndex);

        var renderInformation = RenderInformation[renderIndex];
        int instanceCount = EntityManager.GetBuffer<SpriteIndexBuffer>(renderInformation.renderGroup).Length;
        if (instanceCount <= 0) 
            return instanceCount;
        
        int stride = instanceCount >= 16 ? 16 : 16 * m_spriteSheetCache.GetLength(renderInformation.material);
        if (renderInformation.updateUvs)
        {
            ReleaseUvBuffer(renderIndex);
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

        // if (renderInformation.renderGroup.Stringify().Contains("Select"))
        // {
        //     DebugExtensions.LogVar(new
        //     {
        //         anim = renderInformation.renderGroup.Stringify(),
        //         idxs = renderInformation.indexBuffer.GetData<int>().Stringify(),
        //         matrixB = renderInformation.matrixBuffer.GetData<float4x4>().Stringify(),
        //         uvs = UvBuffer.GetUV(renderInformation.renderGroup).Stringify(),
        //         // args = renderInformation.argsBuffer.GetData<uint>(),
        //         colors = renderInformation.colorsBuffer.GetData<float4>().Stringify(),
        //     }, addCurrFrame:true);
        // }
        return instanceCount;
    }

    private void ReleaseUvBuffer(int bufferID)
    {
        if (RenderInformation[bufferID].uvBuffer != null)
            RenderInformation[bufferID].uvBuffer.Release();
    }

    private void ReleaseBuffer(int bufferID)
    {
        if (RenderInformation[bufferID].matrixBuffer != null)
            RenderInformation[bufferID].matrixBuffer.Release();
        if (RenderInformation[bufferID].colorsBuffer != null)
            RenderInformation[bufferID].colorsBuffer.Release();
        // if(RenderInformation[bufferID].uvBuffer != null)
        //     RenderInformation[bufferID].uvBuffer.Release();
        if (RenderInformation[bufferID].indexBuffer != null)
            RenderInformation[bufferID].indexBuffer.Release();
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