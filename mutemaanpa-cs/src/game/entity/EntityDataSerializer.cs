using System;
using Godot;

namespace Mutemaanpa;

public interface EntityDataSerializer<T>
{
    Variant Serialize(T value);
    T Deserialize(Variant value);
}

public class SimpleSerializer<[MustBeVariant] T> : EntityDataSerializer<T>
{
    public T Deserialize(Variant value)
    {
        return value.As<T>();
    }

    public Variant Serialize(T value)
    {
        return Variant.From(value);
    }
}

public class EnumSerializer<T> : EntityDataSerializer<T> where T : Enum
{
    public T Deserialize(Variant value)
    {
        return (T)Enum.ToObject(typeof(T), (byte)value.As<int>());
    }

    public Variant Serialize(T value)
    {
        return Variant.CreateFrom((int)(object)value);
    }
}

public class UUIDSerializer : EntityDataSerializer<Guid>
{
    public Guid Deserialize(Variant value)
    {
        return Guid.Parse(value.As<string>());
    }

    public Variant Serialize(Guid value)
    {
        return Variant.From(value.ToString());
    }
}

public class EntityDataSerializers
{
    public static EntityDataSerializer<int> INT = new SimpleSerializer<int>();
    public static EntityDataSerializer<float> FLOAT = new SimpleSerializer<float>();
    public static EntityDataSerializer<double> DOUBLE = new SimpleSerializer<double>();
    public static EntityDataSerializer<bool> BOOL = new SimpleSerializer<bool>();
    public static EntityDataSerializer<string> STRING = new SimpleSerializer<string>();
    public static EntityDataSerializer<Guid> UUID = new UUIDSerializer();
    public static EntityDataSerializer<T> ENUM<T>() where T : Enum { return new EnumSerializer<T>(); }
}