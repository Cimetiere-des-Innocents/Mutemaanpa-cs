using System;
using Godot;

namespace Mutemaanpa;

public interface EntityDataSerializer<T>
{
    Variant? Serialize(T? value);
    T? Deserialize(Variant? value);
}

class IntSerializer : EntityDataSerializer<int>
{
    public int Deserialize(Variant? value)
    {
        return value?.As<int>() ?? 0;
    }

    public Variant? Serialize(int value)
    {
        return Variant.CreateFrom(value);
    }
}

class FloatSerializer : EntityDataSerializer<float>
{
    public float Deserialize(Variant? value)
    {
        return value?.As<float>() ?? 0;
    }

    public Variant? Serialize(float value)
    {
        return Variant.CreateFrom(value);
    }
}

class DoubleSerializer : EntityDataSerializer<double>
{
    public double Deserialize(Variant? value)
    {
        return value?.As<double>() ?? 0;
    }

    public Variant? Serialize(double value)
    {
        return Variant.CreateFrom(value);
    }
}

class BoolSerializer : EntityDataSerializer<bool>
{
    public bool Deserialize(Variant? value)
    {
        return value?.As<bool>() ?? false;
    }

    public Variant? Serialize(bool value)
    {
        return Variant.CreateFrom(value);
    }
}

public class EntityDataSerializers
{
    public static EntityDataSerializer<int> INT = new IntSerializer();
    public static EntityDataSerializer<float> FLOAT = new FloatSerializer();
    public static EntityDataSerializer<double> DOUBLE = new DoubleSerializer();
    public static EntityDataSerializer<bool> BOOL = new BoolSerializer();
}