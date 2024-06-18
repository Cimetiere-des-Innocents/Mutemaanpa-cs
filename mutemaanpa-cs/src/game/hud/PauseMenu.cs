namespace Mutemaanpa;

using Godot;

public partial class PauseMenu : CenterContainer
{
    [Export]
    Button? BackGame;

    [Export]
    Button? ToTitle;

    [Export]
    Button? QuitGame;

    public static PauseMenu CreatePauseMenu()
    {
        var node = ResourceLoader.Load<PackedScene>("res://scene/game/hud/pause_menu.tscn")
            .Instantiate<PauseMenu>();
        return node;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        BackGame!.Pressed += () =>
        {
            Router.Of(this).Pop();
        };
        ToTitle!.Pressed += () =>
        {
            GetTree().Quit();
        };
        QuitGame!.Pressed += () =>
        {
            GetTree().Quit();
        };
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
