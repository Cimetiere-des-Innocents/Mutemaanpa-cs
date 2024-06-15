namespace Mutemaanpa;

using Godot;

public partial class MainMenu : VBoxContainer
{
    [Export]
    private Button? _QuitButton;

    [Export]
    private Button? _LoadGameButton;

    [Export]
    private Button? _SettingButton;

    [Export]
    private Button? _NewGameButton;

    private bool _HasSave = false;

    public static MainMenu CreateMainMenu(bool hasSave)
    {
        var node = ResourceLoader.Load<PackedScene>("res://scene/tool/main_menu.tscn")
            .Instantiate<MainMenu>();
        node._HasSave = hasSave;
        return node;
    }

    public override void _Ready()
    {
        base._Ready();
        _QuitButton!.Pressed += () => GetTree().Quit();
        _SettingButton!.Pressed += () => Router.Of(this).Push("/setting");
        _NewGameButton!.Pressed += () => Router.Of(this).Push("/newGame");
        _LoadGameButton!.Disabled = !_HasSave;
    }
}

