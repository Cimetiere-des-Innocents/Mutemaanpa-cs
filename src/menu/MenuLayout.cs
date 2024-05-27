using Godot;
using System;

public partial class MenuLayout : VBoxContainer
{
    [Export]
    private Button _QuitButton;

    [Export]
    private Button _LoadGameButton;

    public override void _Ready()
    {
        base._Ready();
        _QuitButton.Pressed += () => GetTree().Quit();
    }
}
