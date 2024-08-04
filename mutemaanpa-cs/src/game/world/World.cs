namespace Mutemaanpa;

using Godot;

public partial class World : Node3D
{
    [Export]
    Area3D? DeadArea;

    public static World CreateWorld()
    {
        var node = ResourceLoader.Load<PackedScene>("res://scene/game/world/world.tscn")
            .Instantiate<World>();
        return node;
    }
}
