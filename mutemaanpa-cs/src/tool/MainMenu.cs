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
		_NewGameButton!.Pressed += () =>
		{
			var saveDatabase = Provider.Of<SaveDatabase>(this);
			var saveUuid = saveDatabase.NewSave();

			var metadata = Provider.Of<MetadataManager>(this);
			metadata.CurrentSave = saveUuid;

			var characterDb = new CharacterDatabase($"Data Source=m8a_save_{saveUuid}.db");
			characterDb.InitDatabase();
			Router.Of(this).Push("/game");
		};
	}
}

