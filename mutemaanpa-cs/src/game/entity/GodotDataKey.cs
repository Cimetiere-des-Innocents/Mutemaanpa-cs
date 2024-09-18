using Godot;

namespace Mutemaanpa;

public abstract class GodotDataKey(string name)
{
    public readonly string Name = name;
    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }

    public abstract Variant Serialize(Node3D node);

    public abstract void Deserialize(Variant value, Node3D node);
}

class TransformDataKey : GodotDataKey
{
    public TransformDataKey() : base("transform") { }

    public override void Deserialize(Variant value, Node3D node)
    {
        node.Transform = SaveUtil.LoadTransform(value.As<SaveList>());
    }

    public override Variant Serialize(Node3D node)
    {
        return SaveUtil.SaveTransform(node.Transform);
    }
}

class GodotDataKeys
{
    public static readonly GodotDataKey TRANSFORM = new TransformDataKey();
}