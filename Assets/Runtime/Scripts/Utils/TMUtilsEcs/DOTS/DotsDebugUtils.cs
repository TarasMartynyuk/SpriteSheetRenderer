using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public static class DotsDebugUtils
{
    // public static string GetEnityInfo(Entity entity)
    // {
    //     var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    //     var entityManagerDebug = new EntityManager.EntityManagerDebug(entityManager);
    //     return entityManagerDebug.GetEntityInfo(entity);
    // }
    //
    // public static List<object> GetAllComponents(Entity entity)
    // {
    //     var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    //     var componentTypes = entityManager.GetComponentTypes(entity);
    //     var result = new List<object>();
    //     var entityManagerDebug = new EntityManager.EntityManagerDebug(entityManager);
    //     foreach (var type in componentTypes)
    //     {
    //         if (type.IsBuffer)
    //             continue;
    //
    //         try
    //         {
    //             result.Add(entityManagerDebug.GetComponentBoxed(entity, type));
    //         }
    //         catch (Exception e)
    //         {
    //             Debug.LogError($"GetComponentBoxed threw {e.GetType()} for type {type}");
    //         }
    //     }
    //     return result;
    // }
    //
    // public static string GetEntityComponentValues(Entity entity, ComponentType[] componentTypesArg = null)
    // {
    //     var eManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    //
    //     NativeArray<ComponentType> componentTypes;
    //     if (componentTypesArg == null)
    //         componentTypes = eManager.GetComponentTypes(entity);
    //     else
    //         componentTypes = new NativeArray<ComponentType>(componentTypesArg, Allocator.Temp);
    //
    //     var entityManagerDebug = new EntityManager.EntityManagerDebug(eManager);
    //
    //     var sb = new StringBuilder();
    //     var converters = new JsonConverter[] { new Int2JsonConverter(), new Float3JsonConverter() };
    //
    //     foreach (var type in componentTypes)
    //     {
    //         try
    //         {
    //             object component = entityManagerDebug.GetComponentBoxed(entity, type);
    //             sb.AppendLine($"{type}: {JsonConvert.SerializeObject(component, converters)}");
    //         }
    //         catch (Exception e)
    //         {
    //             Debug.LogWarning($"GetEntityComponentValues threw {e.GetType()} for type {type}: {e}");
    //         }
    //     }
    //     return sb.ToString();s
    // }
    //
    // public static void PrintArchetypeAndComponentValues(Entity entity, string label = null, params ComponentType[] components)
    // {
    //     var archetypeInfo = GetEnityInfo(entity);
    //     var eManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    //     components = components.Length == 0 ? null : components;
    //     Debug.Log($"{label}: {eManager.GetNameEditor(entity)} \n {archetypeInfo} \n {GetEntityComponentValues(entity, components)}");
    // }
    //
    // public static void PrintComponentValues(Entity entity, string label = null, params ComponentType[] components)
    // {
    //     var eManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    //     Debug.Log($"{label}: {eManager.GetNameEditor(entity)} \n {GetEntityComponentValues(entity, components)}");
    // }

}
