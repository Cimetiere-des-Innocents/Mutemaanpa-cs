using System;
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
    WorldHud? worldHud;
    Router? router;

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
        AddWorldHud(bindPlayerInfo);
        AddPauseMenu();
        LoadLevel();
    }

    private void AddRouter()
    {
        router = new Router();
        AddChild(router);
        router.Register(("/intermission/opening", () => OpeningScene.CreateOpeningScene(() =>
        {
            router.Overwrite(World.CreateWorld());
            worldHud!.Show();
        })));
    }

    private void AddPauseMenu()
    {
        pauseMenu = PauseMenu.CreatePauseMenu();
        AddChild(pauseMenu);
        pauseMenu.Hide();
    }

    private void AddWorldHud(Action playerCallback)
    {
        worldHud = WorldHud.CreateWorldHud(playerCallback);
        worldHud.MouseFilter = MouseFilterEnum.Pass;
        AddChild(worldHud);
    }

    private void LoadLevel()
    {
        router!.Push("/intermission/opening");
        worldHud!.Hide();
    }

    private static void bindPlayerInfo()
    {

    }
}
