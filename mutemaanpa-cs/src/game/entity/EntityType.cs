using System;
using Godot;

namespace Mutemaanpa;

public class EntityType<T>(string name, Func<T> factory) where T : Entity<Node3D>
{
    public readonly string Name = name;
    public readonly Func<T> Factory = factory;
};

public class EntityTypeUtil
{
    public static EntityType<T> Create<T>(string name, Func<T> factory) where T : Entity<Node3D>
    {
        return new EntityType<T>(name, factory);
    }
}
