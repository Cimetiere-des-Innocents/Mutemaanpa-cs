using System;
using Godot;
using Kernel;

namespace Mutemaanpa;

enum Level
{
    OPENING,
    GLOBULIN
}

/// <summary>
/// GameMain holds the game session state of Mutemaanpa.
/// </summary>
public partial class GameMain : PanelContainer
{
    public Session? Session { get; set; }
    public Journal? Journal { get; set; }
    Action? Saver;
    PauseMenu? pauseMenu;
    Router? router;
    DialogueBox? dialogueBox;

    public static GameMain CreateGameMain(Session session,
                                          Action saver)
    {
        var gameMain = ResourceLoader.Load<PackedScene>("res://scene/game/game_main.tscn")
            .Instantiate<GameMain>();
        gameMain.Session = session;
        gameMain.Journal = new Journal($"m8a_save_{session.id}.db");
        gameMain.Saver = saver;
        return gameMain;
    }

    public static GameMain Of(Node node) => node switch
    {
        GameMain gameMain => gameMain,
        Node another when another.GetParent() is not null => Of(another.GetParent()),
        _ => throw new Exception("I can't find a proper game main!")
    };

    public override void _Ready()
    {
        base._Ready();
        AddRouter();
        AddPauseMenu();
        AddGameOverScene();
        LoadLevel();
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        Session!.update();
    }

    private void AddRouter()
    {
        router = new Router();
        AddChild(router);
        Node GetOpeningScene() => OpeningScene.CreateOpeningScene(onFinished: () =>
        {
            SetLevel(Level.GLOBULIN);
            router.Overwrite(World.CreateWorld());
        });
        router.Register(("/intermission/opening", GetOpeningScene));
        router.Register(("/globulin", World.CreateWorld));
    }

    private void AddPauseMenu()
    {
        pauseMenu = PauseMenu.CreatePauseMenu(SaveGame, SaveGame);
        AddChild(pauseMenu);
        pauseMenu.Hide();
    }

    private void SaveGame()
    {
        Journal!.Store();
        Saver!.Invoke();
    }

    private void LoadLevel()
    {
        switch (GetLevel())
        {
            case Level.OPENING:
                router!.Push("/intermission/opening");
                break;
            case Level.GLOBULIN:
                router!.Overwrite("/globulin");
                break;
        }
    }

    private void AddGameOverScene()
    {
        EventBus.Subscribe<DeadEvent>(GameOverHandler);
    }

    private void GameOverHandler(DeadEvent dead)
    {
            return;
    }

    /// <summary>
    /// When game is over, cleanup all subscriptions.
    /// </summary>
    /// <param name="what"></param>
    public override void _Notification(int what)
    {
        if (what == NotificationPredelete)
        {
            EventBus.Unsubscribe<DeadEvent>(GameOverHandler);
        }
    }


    public void AddDialogueBox(IDialogue dialogue)
    {
        dialogueBox = DialogueBox.CreateDialogueBox(dialogue);
        AddChild(dialogueBox);
    }

    public void RemoveDialogueBox()
    {
        dialogueBox!.QueueFree();
        RemoveChild(dialogueBox);
        dialogueBox = null;
    }

    private void SetGlobalFlag(string key, string value)
    {
        Journal!.Set(Session!.id, key, value);
    }

    private string? GetGlobalFlag(string key) => Journal!.Get(Session!.id, key);

    private Level GetLevel() => GetGlobalFlag("level") switch
    {
        "globulin" => Level.GLOBULIN,
        _ => Level.OPENING
    };

    private void SetLevel(Level level)
    {
        void SetLevelFlag(string value)
        {
            SetGlobalFlag("level", value);
        }
        switch (level)
        {
            case Level.OPENING:
                SetLevelFlag("opening");
                break;
            case Level.GLOBULIN:
                SetLevelFlag("globulin");
                break;
        }
    }
}
