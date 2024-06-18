namespace Mutemaanpa;

using Godot;

public partial class World : Node3D
{

    public static World CreateWorld()
    {
        var node = ResourceLoader.Load<PackedScene>("res://scene/game/world/world.tscn")
            .Instantiate<World>();
        return node;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_cancel"))
        {
            Router.Of(this).Push(PauseMenu.CreatePauseMenu());
        }
    }
}
