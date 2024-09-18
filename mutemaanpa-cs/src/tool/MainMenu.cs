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


    public static MainMenu CreateMainMenu()
    {
        var node = ResourceLoader.Load<PackedScene>("res://scene/tool/main_menu.tscn")
            .Instantiate<MainMenu>();
        return node;
    }

    public override void _Ready()
    {
        base._Ready();
        _QuitButton!.Pressed += () => GetTree().Quit();
        _SettingButton!.Pressed += () =>
        {
            Router.Of(this).Push("/setting");
        };
        _NewGameButton!.Pressed += () => Router.Of(this).Push("/newGame");
        _LoadGameButton!.Pressed += () => Router.Of(this).Push("/load");
    }

    public override void _EnterTree()
    {
        var musicPlayer = MusicPlayer.Of(this);
        if (musicPlayer.Status != "MainMenu")
        {
            musicPlayer.Status = "MainMenu";
            musicPlayer.Stop();
            musicPlayer.Stream = GD.Load<AudioStream>("res://asset/music/mutemaanpa.ogg");
            musicPlayer.PlayDelayed(1.5);
        }
    }
}
