using System;
using System.Reflection;
using Godot;

namespace Mutemaanpa;

using BaseEntityType = EntityType<Entity<Node3D>>;

public class EntityType<T>(string name, Func<T> factory) where T : Entity<Node3D>
{
    public readonly string Name = name;
    public readonly Func<T> Factory = factory;
}

[AttributeUsage(AttributeTargets.Class)]
public class EntityAttribute(BaseEntityType entityType) : Attribute
{
    public readonly BaseEntityType EntityType = entityType;
}

public class EntityTypeUtil
{
    public static EntityType<T> Create<T>(string name, Func<T> factory) where T : Entity<Node3D>
    {
        return new EntityType<T>(name, factory);
    }

    public static BaseEntityType FromScene(string name, string scenePath)
    {
        return new BaseEntityType(name, () => ResourceLoader.Load<PackedScene>(scenePath).Instantiate<Entity<Node3D>>());
    }

    public static BaseEntityType Reflect(Entity<Node3D> entity)
    {
        var attribute = entity.GetType().GetCustomAttribute<EntityAttribute>();
        if (attribute == null)
        {
            throw new Exception($"Cannot find entity type of class {entity.GetType().Name}");
        }
        return attribute.EntityType;
    }
}