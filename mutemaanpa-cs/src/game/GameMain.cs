using Godot;

namespace Mutemaanpa;

/// <summary>
/// The provider & router of the game scene.
/// 
/// NOTE: the save file must be provided in advance
/// </summary>
public partial class GameMain : PanelContainer, IProvider
{
    private readonly Provider provider = new();
    public Provider GetProvider()
    {
        return provider;
    }

    public static GameMain CreateGameMain(CharacterManager characterManager)
    {
        var gameMain = ResourceLoader.Load<PackedScene>("res://scene/game/game_main.tscn")
            .Instantiate<GameMain>();
        gameMain.ResolveDependency(characterManager);
        return gameMain;
    }

    private void ResolveDependency(CharacterManager characterManager)
    {
        provider.Add<CharacterManager>(characterManager);
    }

    public override void _Ready()
    {
        base._Ready();
        AddRouter();
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

}
