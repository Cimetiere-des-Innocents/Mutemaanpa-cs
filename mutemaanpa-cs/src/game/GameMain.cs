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
    /// <summary>
    /// worldHud put here, because it must have a Control parent, otherwise mouse events will not
    /// propagate and the whole ui breaks.
    /// </summary>
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
        AddWorldHud(BindPlayerInfo);
        AddPauseMenu();
        LoadLevel();
    }

    private void AddRouter()
    {
        router = new Router();
        AddChild(router);
        Node GetOpeningScene() => OpeningScene.CreateOpeningScene(() =>
        {
            router.Overwrite(World.CreateWorld());
            worldHud!.Show();
        });
        router.Register(("/intermission/opening", GetOpeningScene));
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

    private void BindPlayerInfo()
    {
        var info = CharacterInformation.From(characterMemory!.GetPlayer());
        worldHud!.AddChild(info);
    }
}
