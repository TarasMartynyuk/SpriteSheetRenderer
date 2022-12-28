using Unity.Entities;

namespace SmokGnu.SpriteSheetRenderer.Utils.TMUtilsEcs
{
    public static class EcsSystemUtils
    {
        public static T CreateSimulationSystem<T>()
            where T : ComponentSystemBase, new()
            => CreateSystemManaged<T, SimulationSystemGroup>();
        
        public static T CreateSystemManaged<T, TSystemGroup>()
            where T : ComponentSystemBase, new()
            where TSystemGroup : ComponentSystemGroup
        {
            var system = World.DefaultGameObjectInjectionWorld.CreateSystemManaged<T>();
            AddSystemForUpdate<T, TSystemGroup>(system);
            return system;
        }

        public static DynamicBuffer<TReinterpret> GetReinterprettedBuffer<TReinterpret, T>(this SystemBase system, Entity entity)
            where T : unmanaged, IBufferElementData
            where TReinterpret : unmanaged
            => system.GetBuffer<T>(entity).Reinterpret<TReinterpret>();

        private static void AddSystemForUpdate<T, TSystemGroup>(T system)
            where T : ComponentSystemBase
            where TSystemGroup : ComponentSystemGroup
        {
            var defaultWorld = World.DefaultGameObjectInjectionWorld;
            var systemGroup = defaultWorld.GetOrCreateSystemManaged<TSystemGroup>();
            systemGroup.AddSystemToUpdateList(system);
        }
    
    }
}