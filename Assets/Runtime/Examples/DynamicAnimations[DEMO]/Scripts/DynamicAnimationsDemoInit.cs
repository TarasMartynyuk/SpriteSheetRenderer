using SmokGnu.SpriteSheetRenderer;
using SmokGnu.SpriteSheetRenderer.Render;
using SmokGnu.SpriteSheetRenderer.ScriptableObject;
using SmokGnu.SpriteSheetRenderer.Utils.TMUtilsEcs;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace SpriteSheetRendererExamples
{
    
    //MY_CMP_ERROR
    public class DynamicAnimationsDemoInit : MonoBehaviour
    {
        [SerializeField] private Shader _shader;
        
        public static Entity character;
        public SpriteSheetAnimator animator;
        public Shader Shader => _shader;
        private static EntityManager EntityManager => World.DefaultGameObjectInjectionWorld.EntityManager;

        private void Awake()
        {
            SpriteSheetRendererInit.Init(Shader);

            var factory3D = new Entity3DFactory();
            var sFactory = new SpriteSheetFactory(factory3D);
            // SpriteSheetCache.Instance.Init(authoring.Shader);

            var spriteSheetRenderSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<SpriteSheetRenderSystem>();
            spriteSheetRenderSystem.RecordAnimator(animator);
        
        
            // 4) Instantiate the entity
            // character = eManager.CreateEntity(archetype);


            character = EntityManager.CreateEntity(sFactory.AnimatedSprite3DDefinition.Archetype);
            sFactory.InitAnimatedSprite(character, animator);
            //eManager.SetName(character, "DynamicAnimationsDemo");
            // 3) Populate components
            var trs = LocalTransform.FromPositionRotationScale(new float3(15), quaternion.identity, 1);
            EntityManager.AddComponentData(character, trs);
            var pts = new PostTransformScale() { Value = float3x3.Scale(2) };
            // var vec = pts.Value.GetScale();
            EntityManager.AddComponentData(character, pts);
            // var color = Color.white;
            // eManager.AddComponentData(character, new Translation { Value = new float3(15) });
            // cmp_err
            // eManager.AddComponentData(character, new NonUniformScale { Value = new float3(-3, 7, 0) });
            // eManager.AddComponentData(character, new SpriteSheetColor { Value = new float4(color.r, color.g, color.b, color.a) });
        
            SpriteMovement.Sprite = character;
        }
    }
}