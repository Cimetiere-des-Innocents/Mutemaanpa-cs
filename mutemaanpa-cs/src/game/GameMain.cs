using System;
using Godot;

namespace Mutemaanpa;

/// <summary>
/// GameMain holds the game session state of Mutemaanpa.
/// </summary>
public partial class GameMain : PanelContainer
{
    public CharacterMemory? CharacterMemory { get; set; }
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
        gameMain.CharacterMemory = characterMemory;
        return gameMain;
    }

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
        Node GetOpeningScene() => OpeningScene.CreateOpeningScene(() =>
        {
            router.Overwrite(World.CreateWorld());
            worldHud!.Show();
        });
        router.Register(("/intermission/opening", GetOpeningScene));
    }

    private void AddPauseMenu()
    {
        pauseMenu = PauseMenu.CreatePauseMenu(SaveGame, SaveGame);
        AddChild(pauseMenu);
        pauseMenu.Hide();
    }

    private void SaveGame()
    {
        CharacterMemory!.Store();
    }

    private void AddWorldHud(Action playerCallback, MemberLiveData memberLiveData)
    {
        worldHud = WorldHud.CreateWorldHud(playerCallback, memberLiveData);
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

    public override void _Notification(int what)
    {
        if (what == NotificationPredelete)
        {
            EventBus.Unsubscribe<DeadEvent>(GameOverHandler);
        }
    }
}
