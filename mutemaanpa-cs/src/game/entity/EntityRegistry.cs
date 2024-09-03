using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;

namespace Mutemaanpa;

using EntityTypeDict = Dictionary<string, EntityType<Entity<Node3D>>>;

public class EntityRegistryEvent(EntityTypeDict entities) : EventArgs
{
    private EntityTypeDict entities = entities;
    public void Register(EntityType<Entity<Node3D>> entityType)
    {
        entities[entityType.Name] = entityType;
    }
}

public class EntityRegistry
{
    private EntityRegistry() { }
    public static readonly EntityRegistry INSTANCE = new EntityRegistry();

    private EntityTypeDict entities = new();

    public void emitRegistryEvent()
    {
        EventBus.Publish(new EntityRegistryEvent(entities));
    }

    public EntityType<Entity<Node3D>> this[string name]
    {
        get { return entities[name]; }
    }
}

public class EntityRegistryBuilder
{
    private EntityRegistryBuilder() { }
    public static readonly EntityRegistryBuilder INSTANCE = new EntityRegistryBuilder();

    public void registerAllEntities()
    {
        EventBus.Subscribe<EntityRegistryEvent>((registryEvent) =>
        {
            var assembly = Assembly.GetExecutingAssembly();
            var classes = assembly.GetTypes();
            var entityClasses = classes.Where(t => t.GetCustomAttributes<EntityAttribute>().Any()).ToList();
            foreach (var entityClass in entityClasses)
            {
                var entityType = entityClass.GetCustomAttribute<EntityAttribute>()!.EntityType;
                registryEvent.Register(entityType);
            }
        });
    }
}