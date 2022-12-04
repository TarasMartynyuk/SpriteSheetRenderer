using TMUtils.Utils.Collections;
using TMUtilsEcs.DOTS.Factories;
using Unity.Entities;
using UnityEngine;

public class SpriteSheetFactory
{
    private static EntityManager EntityManager => World.DefaultGameObjectInjectionWorld.EntityManager;
    private readonly Entity3DFactory _entity3DFactory;
    
    public ComponentType[] AnimatedSpriteComponentTypes { get; private set; }
    public ComponentType[] StaticSpriteComponentTypes { get; private set; }
    public EntityDefinition AnimatedSprite3DDefinition { get; private set; }
    public EntityDefinition StaticSpriteArchetype { get; private set; }

    public SpriteSheetFactory(Entity3DFactory entity3DFactory)
    {
        _entity3DFactory = entity3DFactory;
        StaticSpriteComponentTypes = new ComponentType[]
        {
            typeof(SpriteIndex), typeof(SpriteSheetColor), typeof(SpriteSheetRenderGroupHookComponent)
        };
        AnimatedSpriteComponentTypes = StaticSpriteComponentTypes.Concat(typeof(SpriteSheetAnimationComponent));
        AnimatedSprite3DDefinition = new EntityDefinition(entity3DFactory.DefinitionScale3D.ComponentTypes.Concat(AnimatedSpriteComponentTypes));
        StaticSpriteArchetype = new EntityDefinition(entity3DFactory.DefinitionScale3D.ComponentTypes.Concat(StaticSpriteComponentTypes)); 
    }
    
    
    
    // public void Init()
    // {
    //     
    // }

    public void InitAnimatedSprite(Entity entity, SpriteSheetAnimator animator = null)
    {
        if (animator != null)
        {
            var startAnim = animator.animations[animator.defaultAnimationIndex];
            SpriteSheetAnimationSystem.SetAnimation(entity, startAnim.RenderGroup);
            // DebugExtensions.LogVar(new { spriteSheetEntity = spriteSheetEntity.Stringify(), animator }, "SpriteSheetManager.Init");
        }

        InitSprite(entity);
    }
    
    public void InitStaticSprite(Entity spriteSheetEntity, StaticSpriteScriptable staticSprite)
    {
        _entity3DFactory.Init3DEntity(spriteSheetEntity);
        RenderGroup.AddToNewRenderGroup(spriteSheetEntity, staticSprite.RenderGroup);
        InitSprite(spriteSheetEntity);
    }

    private void InitSprite(Entity sprite)
    {
        _entity3DFactory.Init3DEntity(sprite);
        EntityManager.SetComponentData(sprite, new SpriteSheetColor {Value = (Vector4) Color.white});
    }
}