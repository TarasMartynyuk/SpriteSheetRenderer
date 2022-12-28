using Unity.Entities;

namespace SmokGnu.SpriteSheetRenderer.Utils.TMUtilsEcs
{
    public struct EntityDefinition
    {
        public readonly ComponentType[] ComponentTypes;
        public readonly EntityArchetype Archetype;

        public EntityDefinition(ComponentType[] componentTypes)
        {
            var eManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            ComponentTypes = componentTypes;
            Archetype = eManager.CreateArchetype(componentTypes);
        }

    }
}
