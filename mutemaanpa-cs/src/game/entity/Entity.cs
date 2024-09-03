global using SaveDict = Godot.Collections.Dictionary<string, Godot.Variant>;
global using SaveList = Godot.Collections.Array<Godot.Variant>;
using System;
using System.Collections.Generic;
using Godot;

namespace Mutemaanpa;


public interface Entity<out T> where T : Node3D
{
    IEntityType<Entity<Node3D>> Type { get; }

    EntityDataMap DataMap { get; }

    void DefineData(EntityDataBuilder builder);

    void Save(SaveDict data);

    void Load(SaveDict data);
};

public abstract class EntityDataKeyBase(string name)
{
    public readonly string Name = name;

    public abstract Variant Serialize(object value);

    public abstract object? Deserialize(Variant value);

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
}

public class EntityDataKey<T>(string name, EntityDataSerializer<T> serializer) : EntityDataKeyBase(name)
{
    public readonly EntityDataSerializer<T> Serializer = serializer;

    public T? this[Entity<Node3D> entity]
    {
        get { return (T?)entity.DataMap.Data[this]; }
        set { entity.DataMap.Data[this] = value; }
    }

    public override object? Deserialize(Variant value)
    {
        return Serializer.Deserialize(value);
    }

    public override Variant Serialize(object value)
    {
        return Serializer.Serialize((T)value);
    }
};

public class EntityDataMap
{
    public Dictionary<EntityDataKeyBase, object?> Data = new();
}

public class EntityUtil
{
    public static EntityDataKey<T> CreateDataKey<T>(string name, EntityDataSerializer<T> serializer)
    {
        return new EntityDataKey<T>(name, serializer);
    }

    public static void SaveCustomData(SaveDict saveDict, Entity<Node3D> entity)
    {
        var customDict = new SaveDict();

        foreach (var i in entity.DataMap.Data)
        {
            if (i.Value == null)
            {
                continue;
            }
            customDict[i.Key.Name] = i.Key.Serialize(i.Value);
        }

        saveDict["custom"] = customDict;
    }

    public static void LoadCustomData(SaveDict saveDict, Entity<Node3D> entity)
    {
        var customDict = saveDict["custom"].As<SaveDict>();
        if (customDict == null)
        {
            throw new Exception("Cannot get custom dict");
        }

        foreach (var i in entity.DataMap.Data.Keys)
        {
            try
            {
                var variant = customDict[i.Name];
                if (variant.VariantType == Variant.Type.Nil)
                {
                    continue;
                }
                entity.DataMap.Data[i] = i.Deserialize(variant);
            }
            catch (KeyNotFoundException)
            {
                continue;
            }
        }
    }
}

public class EntityDataBuilder(Entity<Node3D> entity)
{
    private Entity<Node3D> entity = entity;

    public void define<T>(EntityDataKey<T> key, T value)
    {
        entity.DataMap.Data[key] = value;
    }
}