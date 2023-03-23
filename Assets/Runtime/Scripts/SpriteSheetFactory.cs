using SmokGnu.SpriteSheetRenderer.Animation;
using SmokGnu.SpriteSheetRenderer.Animation.Components;
using SmokGnu.SpriteSheetRenderer.Render.RenderGroup;
using SmokGnu.SpriteSheetRenderer.ScriptableObject;
using SmokGnu.SpriteSheetRenderer.Utils;
using SmokGnu.SpriteSheetRenderer.Utils.TMUtilsEcs;
using Unity.Entities;
using UnityEngine;

namespace SmokGnu.SpriteSheetRenderer
{
    public class SpriteSheetFactory
    {
        private static EntityManager EntityManager => World.DefaultGameObjectInjectionWorld.EntityManager;
        private readonly Entity3DFactory _entity3DFactory;
    
        public EntityDefinition AnimatedSprite3DDefinition { get; }
        public EntityDefinition StaticSpriteArchetype { get; }

        public SpriteSheetFactory(Entity3DFactory entity3DFactory)
        {
            _entity3DFactory = entity3DFactory;
            var staticSpriteComponentTypes = new ComponentType[]
            {
                typeof(SpriteIndex), typeof(SpriteSheetColor), typeof(SpriteSheetRenderGroupHookComponent)
            };
            var animatedSpriteComponentTypes = staticSpriteComponentTypes.Concat(typeof(SpriteSheetAnimationComponent));
            AnimatedSprite3DDefinition = new EntityDefinition(entity3DFactory.DefinitionScale3D.ComponentTypes.Concat(animatedSpriteComponentTypes));
            StaticSpriteArchetype = new EntityDefinition(entity3DFactory.DefinitionScale3D.ComponentTypes.Concat(staticSpriteComponentTypes)); 
        }
    
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
            RenderGroup.AddToNewRenderGroup(spriteSheetEntity, staticSprite.RenderGroup);
            InitSprite(spriteSheetEntity);
        }

        private void InitSprite(Entity sprite)
        {
            _entity3DFactory.Init3DEntity(sprite);
            EntityManager.SetComponentData(sprite, new SpriteSheetColor {Value = (Vector4) Color.white});
        }
    }
}