using TMUtilsEcs.DOTS.Ecs;
using Unity.Assertions;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;
using Assert = UnityEngine.Assertions.Assert;

public static class RenderGroup
{
    static EntityManager EntityManager => World.DefaultGameObjectInjectionWorld.EntityManager;

    public static Entity CreateRenderGroup(float4[] uvs, string name)
    {
        var archetype = EntityManager.CreateArchetype(
            typeof(SpriteIndexBuffer),
            typeof(MatrixBuffer),
            typeof(SpriteColorBufferElement),
            typeof(Material),
            typeof(UvBuffer),
            typeof(RenderedEntityBufferElement),
            typeof(SpriteSheetAnimationDefinitionComponent)
        );
        var renderGroup = EntityManager.CreateEntity(archetype);

#if UNITY_EDITOR
        EntityManager.SetNameIndexed(renderGroup, name);
#endif

        UvBuffer.GetUV(renderGroup).CopyFrom(uvs);
        return renderGroup;
    }

    // public static void ReAddToGroup(Entity entity)
    // {
    //     var eManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    //     var groupHookCmp = eManager.GetComponentData<SpriteSheetRenderGroupHookComponent>(entity);
    //     AddToGroup(groupHookCmp.SpritesheetRenderGroup, entity);
    // }

    public static void AddToNewRenderGroup(Entity entity, Entity renderGroup)
    {
        var groupHookCmpRef = new ComponentReference<SpriteSheetRenderGroupHookComponent>(entity);

        if (groupHookCmpRef.Value().SpritesheetRenderGroup != Entity.Null)
            RemoveFromRenderGroup(entity);

        AddToGroup(renderGroup, entity);
    }

    public static void RemoveFromRenderGroup(Entity entity)
    {
        var hookCmp = EntityManager.GetComponentData<SpriteSheetRenderGroupHookComponent>(entity);
        AssertBuffersSameLength(hookCmp.SpritesheetRenderGroup);

        var renderedEntities = RenderedEntityBufferElement.GetRenderedEntities(hookCmp.SpritesheetRenderGroup);
        var swapped = renderedEntities[^1];
        using (var swappedHookCmpRef = new ComponentReference<SpriteSheetRenderGroupHookComponent>(swapped))
            swappedHookCmpRef.Value().IndexInRenderGroup = hookCmp.IndexInRenderGroup;

        renderedEntities.RemoveAtSwapBack(hookCmp.IndexInRenderGroup);
        EntityManager.GetBuffer<SpriteIndexBuffer>(hookCmp.SpritesheetRenderGroup).RemoveAtSwapBack(hookCmp.IndexInRenderGroup);
        EntityManager.GetBuffer<MatrixBuffer>(hookCmp.SpritesheetRenderGroup).RemoveAtSwapBack(hookCmp.IndexInRenderGroup);
        EntityManager.GetBuffer<SpriteColorBufferElement>(hookCmp.SpritesheetRenderGroup).RemoveAtSwapBack(hookCmp.IndexInRenderGroup);

        hookCmp.SpritesheetRenderGroup = Entity.Null;
        EntityManager.SetComponentData(entity, hookCmp);
    }

    private static void AddToGroup(Entity renderGroup, Entity entity)
    {
        AssertBuffersSameLength(renderGroup);
        var renderedEntities = RenderedEntityBufferElement.GetRenderedEntities(renderGroup);
        Assert.IsFalse(renderedEntities.Contains(entity));

        renderedEntities.Add(entity);
        SpriteIndexBuffer.GetSpriteIndices(renderGroup).Add(EntityManager.GetComponentData<SpriteIndex>(entity).Value);
        MatrixBuffer.GetMatrixBuffer(renderGroup).Add(EntityManager.GetComponentData<LocalToWorld>(entity).Value);
        SpriteColorBufferElement.GetColors(renderGroup).Add(EntityManager.GetComponentData<SpriteSheetColor>(entity).Value);

        var index = renderedEntities.LastIndex();
        var eManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        eManager.SetComponentData(entity,
            new SpriteSheetRenderGroupHookComponent
                {IndexInRenderGroup = index, SpritesheetRenderGroup = renderGroup});
    }


    private static void AssertBuffersSameLength(Entity renderGroup)
    {
        var renderedEntities = RenderedEntityBufferElement.GetRenderedEntities(renderGroup);
        var spriteIndices = SpriteIndexBuffer.GetSpriteIndices(renderGroup);
        var matrices = MatrixBuffer.GetMatrixBuffer(renderGroup);
        var colors = SpriteColorBufferElement.GetColors(renderGroup);

        var length = renderedEntities.Length;
        Debug.Assert(spriteIndices.Length == length &&
                     matrices.Length == length &&
                     colors.Length == length);
    }
}