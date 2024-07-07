namespace Mutemaanpa;

using System;
using Godot;

public partial class PauseMenu : CenterContainer
{
    [Export]
    Button? BackGame;

    [Export]
    Button? ToTitle;

    [Export]
    Button? QuitGame;
    Action? ToTitleCallback;
    Action? QuitCallback;

    public static PauseMenu CreatePauseMenu(Action toMenuCallback, Action quitCallback)
    {
        var node = ResourceLoader.Load<PackedScene>("res://scene/game/hud/pause_menu.tscn")
            .Instantiate<PauseMenu>();
        node.ToTitleCallback = toMenuCallback;
        node.QuitCallback = quitCallback;
        return node;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionReleased("ui_cancel"))
        {
            Visible = !Visible;
            GetTree().Paused = !GetTree().Paused;
            // NOTE that if we don't accept the event, it will propagates to parent
            // controls which can result in double canceling... that may not be desired.
            AcceptEvent();
        }
    }

    public override void _Ready()
    {
        BackGame!.Pressed += () =>
        {
            Hide();
            GetTree().Paused = false;
        };
        ToTitle!.Pressed += () =>
        {
            GetTree().Paused = false;
            ToTitleCallback!.Invoke();
            Router.Of(this).Pop();
        };
        QuitGame!.Pressed += () =>
        {
            QuitCallback!.Invoke();
            GetTree().Quit();
        };
    }

}
