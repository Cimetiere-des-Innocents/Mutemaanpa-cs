using System;
using System.Reflection;
using Godot;

namespace Mutemaanpa;

using BaseEntityType = IEntityType<Entity<Node3D>>;

public interface IEntityType<out T> where T : Entity<Node3D>
{
    public string Name { get; }
    public Func<T> Factory { get; }
}

public class EntityType<T>(string name, Func<T> factory) : IEntityType<T> where T : Entity<Node3D>
{
    private readonly string name = name;
    private readonly Func<T> factory = factory;
    public string Name => name;
    public Func<T> Factory => factory;
}

[AttributeUsage(AttributeTargets.Class)]
public class EntityAttribute : Attribute { }

public class EntityTypeUtil
{
    public static EntityType<T> Create<T>(string name, Func<T> factory) where T : Entity<Node3D>
    {
        return new EntityType<T>(name, factory);
    }

    public static EntityType<T> FromScene<T>(string name, string scenePath) where T : Node3D, Entity<Node3D>
    {
        return new EntityType<T>(name, () => ResourceLoader.Load<PackedScene>(scenePath).Instantiate<T>());
    }

    public static BaseEntityType Reflect(Entity<Node3D> entity)
    {
        var attribute = entity.GetType().GetCustomAttribute<EntityAttribute>();
        if (attribute == null)
        {
            throw new Exception($"Cannot find entity type of class {entity.GetType().Name}");
        }
        var typeField = entity.GetType().GetField("TYPE", BindingFlags.Public | BindingFlags.Static);
        if (typeField == null)
        {
            throw new Exception($"Cannot find entity type of class {entity.GetType().Name}");
        }
        var value = typeField.GetValue(null);
        if (value is BaseEntityType entityType)
        {
            return entityType;
        }
        else
        {
            throw new Exception($"Cannot find entity type of class {entity.GetType().Name}");
        }
    }
}