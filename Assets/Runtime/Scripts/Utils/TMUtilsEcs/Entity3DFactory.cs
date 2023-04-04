using SmokGnu.SpriteSheetRenderer.Utils.TMUtilsEcs.DOTS;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace SmokGnu.SpriteSheetRenderer.Utils.TMUtilsEcs
{
    public class Entity3DFactory
    {
        public EntityDefinition DefinitionScale3D { get; }
        public EntityDefinition Definition { get; }
        private EntityManager EntityManager => World.DefaultGameObjectInjectionWorld.EntityManager;

        public Entity3DFactory()
        {
            Definition = new EntityDefinition(new ComponentType[]
            {
                typeof(LocalTransform), typeof(LocalToWorld)
            });

            DefinitionScale3D =
                new EntityDefinition(Definition.ComponentTypes.Concat(typeof(PostTransformMatrix)));
        }

        public Entity Create3DEntity(string name = "dummy",
            float3? scale3D = null,
            Entity? parent = null)
        {
            var entity = EntityManager.CreateEntity(name, DefinitionScale3D.Archetype);

            Init3DEntity(entity, scale3D);
            if (parent != null)
            {
                EntityManager.SetComponentData(entity, new Parent { Value = parent.GetValueOrDefault(Entity.Null) });
            }

            return entity;
        }

        public void Init3DEntity(Entity entity, float3? scale3D = null)
        {
            EntityManager.SetComponentData(entity, LocalTransform.Identity);

            var scaleMatrix = float4x4.Scale(scale3D.GetValueOrDefault(Float3V.One));
            EntityManager.SetComponentData(entity, new PostTransformMatrix { Value = scaleMatrix });

            // cmp_err?
            // EntityTransformUtils.SetRootPositionUpdateLTW(entity, float3.zero);
        }
    }
}