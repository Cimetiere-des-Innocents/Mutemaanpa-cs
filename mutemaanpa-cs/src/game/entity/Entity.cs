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

    Guid uuid { get; }

    void DefineData(EntityDataBuilder builder);

    public void Save(SaveDict saveDict)
    {
        var godotDict = new SaveDict();

        foreach (var i in DataMap.GodotData)
        {
            if (i.Value)
            {
                godotDict[i.Key.Name] = i.Key.Serialize(Value);
            }
        }

        saveDict["godot"] = godotDict;

        var customDict = new SaveDict();

        foreach (var i in DataMap.Data)
        {
            if (i.Value == null || !DataMap.DoSave[i.Key])
            {
                continue;
            }
            customDict[i.Key.Name] = i.Key.Serialize(i.Value);
        }

        saveDict["custom"] = customDict;
    }

    public void Load(SaveDict saveDict)
    {
        var godotDict = saveDict["godot"].As<SaveDict>();
        if (godotDict != null)
        {
            foreach (var i in DataMap.GodotData)
            {
                if (i.Value)
                {
                    try
                    {
                        var variant = godotDict[i.Key.Name];
                        if (variant.VariantType == Variant.Type.Nil)
                        {
                            continue;
                        }

                        i.Key.Deserialize(variant, Value);
                    }
                    catch (KeyNotFoundException)
                    {
                        continue;
                    }
                }
            }
        }

        var customDict = saveDict["custom"].As<SaveDict>();
        if (customDict != null)
        {
            foreach (var i in DataMap.Data.Keys)
            {
                try
                {
                    var variant = customDict[i.Name];
                    if (variant.VariantType == Variant.Type.Nil)
                    {
                        continue;
                    }

                    if (DataMap.DoSave[i])
                    {
                        DataMap.Data[i] = i.Deserialize(variant);
                    }
                }
                catch (KeyNotFoundException)
                {
                    continue;
                }
            }
        }
    }

    void OnSpawned();

    void OnChunkTick(Chunk chunk);

    void Tick(double delta);

    T Value { get; }
};

public static class EntityExt
{
    public static void Save<T>(Entity<T> e, SaveDict saveDict) where T : Node3D
    {
        e.Save(saveDict);
    }

    public static SaveDict SerializeAll<T>(Entity<T> e) where T : Node3D
    {
        var dict = new SaveDict();
        Save(e, dict);
        return dict;
    }

    public static void Load<T>(Entity<T> e, SaveDict saveDict) where T : Node3D
    {
        e.Load(saveDict);
    }
}

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
    public Dictionary<EntityDataKeyBase, object?> Data = [];
    public Dictionary<EntityDataKeyBase, bool> DoSave = [];
    public Dictionary<GodotDataKey, bool> GodotData = [];
}

public class EntityDataBuilder(Entity<Node3D> entity)
{
    private Entity<Node3D> entity = entity;

    public void define<T>(EntityDataKey<T> key, T value)
    {
        entity.DataMap.Data[key] = value;
        entity.DataMap.DoSave[key] = true;
    }

    public void define(GodotDataKey key)
    {
        entity.DataMap.GodotData[key] = true;
    }

    public void unDefine<T>(EntityDataKey<T> key)
    {
        entity.DataMap.DoSave[key] = false;
    }

    public void unDefine(GodotDataKey key)
    {
        entity.DataMap.GodotData[key] = false;
    }
}

public class EntityUtil
{
    public static readonly EntityDataKey<Guid> UUID = new("uuid", EntityDataSerializers.UUID);

    public static EntityDataKey<T> CreateDataKey<T>(string name, EntityDataSerializer<T> serializer)
    {
        return new EntityDataKey<T>(name, serializer);
    }
}
