using Godot;

namespace Mutemaanpa;

/// <summary>
/// The provider & router of the game scene.
/// 
/// NOTE: the save file must be provided in advance
/// </summary>
public partial class GameMain : PanelContainer
{
    CharacterMemory? characterMemory;

    PauseMenu? pauseMenu;

    public static GameMain CreateGameMain(CharacterMemory characterMemory)
    {
        var gameMain = ResourceLoader.Load<PackedScene>("res://scene/game/game_main.tscn")
            .Instantiate<GameMain>();
        gameMain.characterMemory = characterMemory;
        return gameMain;
    }

    public override void _Ready()
    {
        base._Ready();
        AddRouter();
        AddPauseMenu();
    }

    private void AddRouter()
    {
        var router = Router.CreateRouter(
            defaultPage: "/intermission/opening",
            routes: [
                (name: "/intermission/opening", uri: "res://scene/game/intermission/opening_scene.tscn")
            ]
        );
        AddChild(router);
    }

    private void AddPauseMenu()
    {
        pauseMenu = PauseMenu.CreatePauseMenu();
        AddChild(pauseMenu);
        pauseMenu.Hide();
    }
}
