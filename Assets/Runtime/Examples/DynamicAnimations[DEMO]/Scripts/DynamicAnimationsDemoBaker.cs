using TMUtilsEcs.DOTS.Factories;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace SpriteSheetRendererExamples
{
    public class DynamicAnimationsDemoBaker// : Baker<DynamicAnimationsDemoInit>
    {
        public void Bake(DynamicAnimationsDemoInit init)
        {
            //eManager.SetNameInd(entity, "CONVERt ENTITY");
        
            // 1) Create Archetype
            // EntityArchetype archetype = eManager.CreateArchetype(
            //          typeof(LocalToWorld),
            //          typeof(Translation),
            //          typeof(Rotation),
            //          typeof(NonUniformScale),
            //          //required params
            //          typeof(SpriteIndex),
            //          typeof(SpriteSheetAnimationComponent),
            //          typeof(Material),
            //          typeof(SpriteSheetColor),
            //          typeof(SpriteSheetRenderGroupHookComponent)
            // );
            
            // SpriteSheetRendererInit.Init(init.Shader);
            //
            // var factory3D = new Entity3DFactory();
            // var sFactory = new SpriteSheetFactory(factory3D);
            // // SpriteSheetCache.Instance.Init(authoring.Shader);
            //
            // var spriteSheetRenderSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<SpriteSheetRenderSystem>();
            // spriteSheetRenderSystem.RecordAnimator(init.animator);
            //
            //
            // // 4) Instantiate the entity
            // // character = eManager.CreateEntity(archetype);
            //
            //
            // var eManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            // DynamicAnimationsDemoInit.character = eManager.CreateEntity(sFactory.AnimatedSpriteComponentTypes);
            // sFactory.InitAnimatedSprite(DynamicAnimationsDemoInit.character, init.animator);
            // //eManager.SetName(character, "DynamicAnimationsDemo");
            // // 3) Populate components
            // var trs = LocalTransform.FromPositionRotationScale(new float3(15), quaternion.identity, 1);
            // eManager.AddComponentData(DynamicAnimationsDemoInit.character, trs);
            // // var color = Color.white;
            // // eManager.AddComponentData(character, new Translation { Value = new float3(15) });
            // // cmp_err
            // // eManager.AddComponentData(character, new NonUniformScale { Value = new float3(-3, 7, 0) });
            // // eManager.AddComponentData(character, new SpriteSheetColor { Value = new float4(color.r, color.g, color.b, color.a) });
            //
            // SpriteMovement.Sprite = DynamicAnimationsDemoInit.character;
        }
    }
}