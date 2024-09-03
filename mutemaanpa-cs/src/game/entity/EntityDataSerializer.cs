using Godot;

namespace Mutemaanpa;

public interface EntityDataSerializer<T>
{
    Variant Serialize(T value);
    T Deserialize(Variant value);
}