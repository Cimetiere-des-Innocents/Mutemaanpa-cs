using Godot;
using Mutemaanpa;

public partial class MenuLayout : VBoxContainer
{
    [Export]
    private Button _QuitButton;

    [Export]
    private Button _LoadGameButton;

    [Export]
    private Button _SettingButton;

    public override void _Ready()
    {
        base._Ready();
        _QuitButton.Pressed += () => GetTree().Quit();
        _SettingButton.Pressed += () => Router.Of(this).Switch("/setting");
    }
}
