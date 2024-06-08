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

	public override void _Ready()
	{
		base._Ready();
		_QuitButton!.Pressed += () => GetTree().Quit();
		_SettingButton!.Pressed += () => Router.Of(this).Push("/setting");
		_NewGameButton!.Pressed += () => Router.Of(this).Push("/newGame");
	}
}

