using Godot;

namespace Mutemaanpa;

using SaveDict = Godot.Collections.Dictionary<string, Variant>;

public interface Entity<out T> where T : Node3D
{
    void Save(SaveDict data);
    void Load(SaveDict data);
};

public class EntityHelper
{
    void SaveTransform(SaveDict data, Node3D node)
    {
        data["transform"] = node.Transform;
    }
}