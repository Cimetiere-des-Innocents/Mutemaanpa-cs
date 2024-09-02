using System;
using System.Text.Json;
using Godot;

namespace Mutemaanpa;

public enum Level
{
    OPENING,
    CREATE,
    GLOBULIN
}

/// <summary>
/// GameMain holds the game session state of Mutemaanpa.
/// </summary>
public partial class GameMain : PanelContainer
{
    public static readonly string SAVE_FILE = "/mainGame.json";
    public DirAccess? Save { get; set; }
    Level? GameLevel { get; set; }

    PauseMenu? pauseMenu;
    Node? world;

    private static string GetSaveDirname(Guid save)
    {
        return $"m8a_{save}";
    }

    /// <summary>
    /// CreateGameMain loads the game if save exists, create a new one if save does not exist.
    /// </summary>
    /// <param name="save"></param>
    /// <returns></returns>
    public static GameMain CreateGameMain(Guid save)
    {
        var gameMain = ResourceLoader.Load<PackedScene>("res://scene/game/game_main.tscn")
            .Instantiate<GameMain>();
        var dir = Catalog.ToGameFS();
        gameMain.Save = dir;
        var saveDir = GetSaveDirname(save);
        if (dir.DirExists(saveDir))
        {
            dir.ChangeDir(saveDir);
            using var file = FileAccess.Open(dir.GetCurrentDir() + SAVE_FILE, FileAccess.ModeFlags.Read);
            gameMain.GameLevel = JsonSerializer.Deserialize<Level>(file.GetAsText());
        }
        else
        {
            gameMain.GameLevel = Level.OPENING;
            dir.MakeDir(saveDir);
            dir.ChangeDir(saveDir);
            gameMain.SaveGame();
        }
        gameMain.SpawnWorld();
        return gameMain;
    }

    void OpeningToCreate(OpeningScene node)
    {
        GameLevel = Level.CREATE;
        RemoveChild(node);
        node.QueueFree();
        SpawnWorld();
    }

    public void CreateToGlobulin(CharacterCreation node, CharacterStat stat, CharacterAbility ability)
    {
        GameLevel = Level.GLOBULIN;
        RemoveChild(node);
        node.QueueFree();
        world = World.CreateWorld(stat, ability);
        AddChild(world);
    }
    private void SpawnWorld()
    {
        switch (GameLevel)
        {
            case Level.OPENING:
                world = OpeningScene.CreateOpeningScene(onFinished: OpeningToCreate);
                break;
            case Level.CREATE:
                world = CharacterCreation.CreateCharacterCreation(CreateToGlobulin);
                break;
            case Level.GLOBULIN:
                world = World.LoadWorld(Save!);
                break;
        }
        AddChild(world);
    }

    public override void _Ready()
    {
        base._Ready();
        AddPauseMenu();
        // AddGameOverScene();
    }

    private void AddPauseMenu()
    {
        pauseMenu = PauseMenu.CreatePauseMenu(SaveGame, SaveGame);
        AddChild(pauseMenu);
        pauseMenu.Hide();
    }

    private void SaveGame()
    {
        using var file = FileAccess.Open(Save!.GetCurrentDir() + SAVE_FILE, FileAccess.ModeFlags.Write);
        var s = JsonSerializer.Serialize(GameLevel!.Value);
        file.StoreLine(s);
    }

    // private void AddGameOverScene()
    // {
    //     EventBus.Subscribe<DeadEvent>(GameOverHandler);
    // }

    // private void GameOverHandler(DeadEvent dead)
    // {
    //     if (dead.Character.Data.Player is null)
    //     {
    //         return;
    //     }
    //     // Notice GameMain cannot collect its garbages, so we need a special scene to do this.
    //     Router.Of(this).Push("/gameOver", removeOld: false);
    // }

    /// <summary>
    /// When game is over, cleanup all subscriptions.
    /// </summary>
    /// <param name="what"></param>
    // public override void _Notification(int what)
    // {
    //     if (what == NotificationPredelete)
    //     {
    //         EventBus.Unsubscribe<DeadEvent>(GameOverHandler);
    //     }
    // }
}
