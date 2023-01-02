using System.Diagnostics;
using Unity.Collections;
using Unity.Entities;

namespace SmokGnu.SpriteSheetRenderer.Utils.TMUtilsEcs.DOTS
{
    internal static class EntityManagerNameExtensions
    {
        private const string kBaseEntityIndexName = "{0}: "; // 0 is entity index
        private const string Null = "Null";

        [Conditional("UNITY_EDITOR")]
        public static void SetNameIndexed(this EntityManager entityManager, Entity entity, string name)
        {
#if UNITY_EDITOR
            entityManager.SetName(entity, string.Format(kBaseEntityIndexName + name, entity.Index));
#endif
        }

        public static string GetNameEditor(this EntityManager eManager, Entity entity)
        {
            if (entity == Entity.Null)
                return Null;

#if UNITY_EDITOR
            return eManager.GetName(entity);
#else
        return string.Empty;
#endif
        }

        public static Entity CreateEntity(this EntityManager entityManager, string name, params ComponentType[] types)
        {
            var entity = entityManager.CreateEntity(types);
            entityManager.SetNameIndexed(entity, name);
            return entity;
        }

        public static Entity CreateEntity(this EntityManager entityManager, string name, EntityArchetype entityArchetype)
        {
            var entity = entityManager.CreateEntity(entityArchetype);
            entityManager.SetNameIndexed(entity, name);
            return entity;
        }

        public static Entity CreateEntity(this EntityManager entityManager, string name)
        {
            var entity = entityManager.CreateEntity();
            entityManager.SetNameIndexed(entity, name);
            return entity;
        }

        public static Entity Instantiate(this EntityManager entityManager, string name, Entity srcEntity)
        {
            var entity = entityManager.Instantiate(srcEntity);
            entityManager.SetNameIndexed(entity, name);
            return entity;
        }

        public static void Instantiate(this EntityManager entityManager, string name, Entity srcEntity,
            NativeArray<Entity> outputEntities)
        {
            entityManager.Instantiate(srcEntity, outputEntities);
            foreach (var entity in outputEntities)
                entityManager.SetNameIndexed(entity, name);
        }
    }
}
