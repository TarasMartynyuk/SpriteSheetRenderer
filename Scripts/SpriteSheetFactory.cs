using Unity.Entities;
using UnityEngine;

public class SpriteSheetFactory : SingletonBase<SpriteSheetFactory>
{
    public ComponentType[] AnimatedSpriteComponentTypes { get; private set; }
    public ComponentType[] StaticSpriteComponentTypes { get; private set; }
    public Entity3DDefinition AnimatedSprite3DDefinition { get; private set; }
    public Entity3DDefinition StaticSpriteArchetype { get; private set; }
    EntityManager EntityManager => World.DefaultGameObjectInjectionWorld.EntityManager;
    
    public void Init()
    {
        StaticSpriteComponentTypes = new ComponentType[]
        {
            typeof(SpriteIndex), typeof(SpriteSheetColor), typeof(SpriteSheetRenderGroupHookComponent)
        };
        AnimatedSpriteComponentTypes = StaticSpriteComponentTypes.Concat(typeof(SpriteSheetAnimationComponent));
        AnimatedSprite3DDefinition = new Entity3DDefinition(AnimatedSpriteComponentTypes);
        StaticSpriteArchetype = new Entity3DDefinition(StaticSpriteComponentTypes); 
    }

    public void InitAnimatedSprite(Entity entity, SpriteSheetAnimator animator = null)
    {
        if (animator != null)
        {
            var startAnim = animator.animations[animator.defaultAnimationIndex];
            SpriteSheetManager.SetAnimation(entity, startAnim.RenderGroup);
            // DebugExtensions.LogVar(new { spriteSheetEntity = spriteSheetEntity.Stringify(), animator }, "SpriteSheetManager.Init");
        }

        InitSprite(entity);
    }
    
    public void InitStaticSprite(Entity spriteSheetEntity, StaticSpriteScriptable staticSprite)
    {
        Entity3DFactory.Instance.Init3DEntity(spriteSheetEntity);
        SpriteSheetManager.ChangeRenderGroup(spriteSheetEntity, staticSprite.RenderGroup, EntityManager);
        InitSprite(spriteSheetEntity);
    }

    private void InitSprite(Entity sprite)
    {
        Entity3DFactory.Instance.Init3DEntity(sprite);
        EntityManager.SetComponentData(sprite, new SpriteSheetColor {Value = (Vector4) Color.white});
    }
}