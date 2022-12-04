using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

namespace TMUtilsEcs.DOTS.Ecs
{
    public ref struct ComponentReference<T> where T : unmanaged, IComponentData
    {
        T m_component;
        Entity m_entity;

        public ComponentReference(Entity entity)
        {
            m_entity = entity;
            m_component = default;
            ReadComponent();
        }

        public ref T Value()
        {
            unsafe
            {
                return ref UnsafeUtility.AsRef<T>(UnsafeUtility.AddressOf(ref m_component));
            }
        }

        public void ReadComponent()
        {
            var eManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            m_component = eManager.GetComponentData<T>(m_entity);
        }

        public void WriteComponent()
        {
            var eManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            eManager.SetComponentData(m_entity, m_component);
        }

        public void Dispose()
        {
            WriteComponent();
        }
    }
}
