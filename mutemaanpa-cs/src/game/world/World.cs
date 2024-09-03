
using Godot;

namespace Mutemaanpa;
public partial class World : Node3D
{
    [Export]
    Area3D? DeadArea;

    [Export]
    Player? player;

    public static World LoadWorld(DirAccess save)
    {
        var node = ResourceLoader.Load<PackedScene>("res://scene/game/world/world.tscn")
            .Instantiate<World>();
        return node;
    }

    public static World CreateWorld(CharacterStat stat, CharacterAbility ability)
    {
        var node = ResourceLoader.Load<PackedScene>("res://scene/game/world/world.tscn")
            .Instantiate<World>();
        node.player = Player.CreatePlayer(stat, ability);
        node.AddChild(node.player);
        return node;
    }
}
