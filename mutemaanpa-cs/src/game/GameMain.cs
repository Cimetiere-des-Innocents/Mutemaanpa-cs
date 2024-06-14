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

    public override void _Ready()
    {
        base._Ready();
        ResolveDependency();
        AddRouter();
    }

    private void ResolveDependency()
    {
        var currentSave = Provider.Of<MetadataManager>(this).CurrentSave;
        if (currentSave is not null)
        {
            var characterDb = new CharacterDatabase($"Data Source=m8a_save_{currentSave}.db");
            provider.Add<CharacterDatabase>(characterDb);

            var characterManager = new CharacterManager(characterDb);
            provider.Add<CharacterManager>(characterManager);
        }
    }

    private void AddRouter()
    {
        var router = Router.CreateRouter(
            defaultPage: "/newGame",
            routes: [
                (name: "/newGame", uri: "res://scene/game/character/character_creation.tscn"),
                (name: "/intermission/opening", uri: "res://scene/game/intermission/opening_scene.tscn")
            ]
        );
        AddChild(router);
    }

}
