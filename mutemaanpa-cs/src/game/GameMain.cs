using System;
using Godot;

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
    public CharacterMemory? CharacterMemory { get; set; }
    public Journal? Journal { get; set; }
    public Guid? Save { get; set; }
    PauseMenu? pauseMenu;
    /// <summary>
    /// worldHud put here, because it must have a Control parent, otherwise mouse events will not
    /// propagate and the whole ui breaks.
    /// </summary>
    WorldHud? worldHud;
    Router? router;
    DialogueBox? dialogueBox;

    public static GameMain CreateGameMain(CharacterMemory characterMemory,
                                          Journal journal,
                                          Guid save)
    {
        var gameMain = ResourceLoader.Load<PackedScene>("res://scene/game/game_main.tscn")
            .Instantiate<GameMain>();
        gameMain.CharacterMemory = characterMemory;
        gameMain.Journal = journal;
        gameMain.Save = save;
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
        AddWorldHud(BindPlayerInfo, BindPlayerHpMp());
        AddPauseMenu();
        AddGameOverScene();
        LoadLevel();
    }


    private void AddRouter()
    {
        router = new Router();
        AddChild(router);
        Node GetOpeningScene() => OpeningScene.CreateOpeningScene(onFinished: () =>
        {
            SetLevel(Level.GLOBULIN);
            router.Overwrite(World.CreateWorld());
            worldHud!.Show();
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
        CharacterMemory!.Store();
    }

    private void AddWorldHud(Action playerCallback, MemberLiveData memberLiveData)
    {
        worldHud = WorldHud.CreateWorldHud(playerCallback, memberLiveData);
        AddChild(worldHud);
    }

    private void LoadLevel()
    {
        switch (GetLevel())
        {
            case Level.OPENING:
                router!.Push("/intermission/opening");
                worldHud!.Hide();
                break;
            case Level.GLOBULIN:
                router!.Overwrite("/globulin");
                worldHud!.Show();
                break;
        }
    }

    private void BindPlayerInfo()
    {
        var info = CharacterInformation.From(CharacterMemory!.GetPlayerState());
        worldHud!.AddChild(info);
    }

    private MemberLiveData BindPlayerHpMp()
    {
        return new MemberLiveData(
            GetMaxHp: () => CharacterMemory!.GetPlayerState().Runtime.MaxHitPoint,
            GetCurHp: () => CharacterMemory!.GetPlayerState().Data.Stat.Hp,
            GetMaxMp: () => CharacterMemory!.GetPlayerState().Runtime.MaxManaPoint,
            GetCurMp: () => CharacterMemory!.GetPlayerState().Data.Stat.Mp
        );
    }

    private void AddGameOverScene()
    {
        EventBus.Subscribe<DeadEvent>(GameOverHandler);
    }

    private void GameOverHandler(DeadEvent dead)
    {
        if (dead.Character.Data.Player is null)
        {
            return;
        }
        // Notice GameMain cannot collect its garbages, so we need a special scene to do this.
        Router.Of(this).Push("/gameOver", removeOld: false);
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
        Journal!.Set(Save!.Value, key, value);
    }

    private string? GetGlobalFlag(string key) => Journal!.Get(Save!.Value, key);

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
