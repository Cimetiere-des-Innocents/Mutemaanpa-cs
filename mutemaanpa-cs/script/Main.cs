namespace Mutemaanpa;

using Godot;

/// <summary>
/// Main class controls the whole game wide configuration / states.
///
/// Main also does dependency injection for Mutemaanpa.
/// https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection
///
/// # Main States
///
/// ----------------------------------------------------
/// |   [Load Game] ------------------------           |
/// |    1^                               6^           |
/// |    2V                               7v        12 |
/// --> [Title] 3<->4 [New Game] ---->5 [In Game]<------
///  13  8^                                 ^
///      9v                                 | 10
///     [Setting] <--------------------------
///               11
///
///
///  1. click "Load Game"
///  2. click "Back To Title"
///  3. click "New Game"
///  4. click "Back To Title"
///  5. click "Start Game"
///  6. click "Load Game"
///  7. click "Start Game"
///  8. click "Ok"
///  9. click "Settings"
///  10. click "Ok"
///  11. click "Settings"
///  12. click "Back To Title"
///  13. click "Continue"
///
/// </summary>
public partial class Main : PanelContainer
{
	public Provider provider = new();
	public override void _Ready()
	{
		base._Ready();
		Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

		var db = new Database("Data Source=m8a_save.db");
		var characterManager = new CharacterManager(db);
		provider.Add<Database>(db);
		provider.Add<CharacterManager>(characterManager);

		var router = Router.CreateRouter(
			defaultPage: "/menu",
			routes: [
				(name: "/menu", uri: "res://scene/ui/main_menu.tscn"),
				(name: "/setting", uri: "res://scene/ui/setting_page.tscn"),
				(name: "/newGame", uri: "res://scene/character/character_creation.tscn"),
			]
		);
		AddChild(router);
	}
}
