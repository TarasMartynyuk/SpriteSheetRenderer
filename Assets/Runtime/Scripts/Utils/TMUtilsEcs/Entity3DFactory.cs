using TMUtils.Singletons;
using TMUtils.Utils.Collections;
using TMUtils.Utils.Math;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace TMUtilsEcs.DOTS.Factories
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
                typeof(LocalTransform),
                typeof(LocalToWorld),
                typeof(WorldTransform),
            });

            DefinitionScale3D =
                new EntityDefinition(Definition.ComponentTypes.Concat(typeof(PostTransformScale)));
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
            
            if (scale3D != null)
            {
                var scaleMatrix = float3x3.identity;
                EntityManager.SetComponentData(entity, new PostTransformScale { Value = scaleMatrix });
            }

            // cmp_err?
            // EntityTransformUtils.SetRootPositionUpdateLTW(entity, float3.zero);
        }
    }
}