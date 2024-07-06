namespace Mutemaanpa;

using System;
using Godot;

public partial class OpeningScene : PanelContainer
{
    Action? onFinished;

    public static OpeningScene CreateOpeningScene(Action onFinished)
    {
        var node = ResourceLoader.Load<PackedScene>("res://scene/game/intermission/opening_scene.tscn")
            .Instantiate<OpeningScene>();
        node.onFinished = onFinished;
        return node;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionReleased("ui_accept") 
            || (@event is InputEventMouseButton m && m.IsReleased()))
        {
            onFinished?.Invoke();
        }
    }
}
