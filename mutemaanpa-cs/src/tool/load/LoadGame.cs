namespace Mutemaanpa;

using Dapper;
using Godot;

public partial class LoadGame : ScrollContainer
{
    [Export]
    VBoxContainer? saveList;

    [Export]
    Button? backButton;

    public static LoadGame CreateLoadGame(SaveDatabase saveDatabase)
    {
        var node = ResourceLoader.Load<PackedScene>("res://scene/tool/load/load_game.tscn")
            .Instantiate<LoadGame>();
        node.backButton!.Pressed += () =>
        {
            Router.Of(node).Pop();
        };
        saveDatabase.QuerySaves().AsList()
            .ForEach((save) => node.saveList!.AddChild(SaveSlot.CreateSaveSlot(save, () =>
        {
            var characterDb = new CharacterDatabase($"Data Source=m8a_save_{save.Id}.db");
            var characterMemory = new CharacterMemory(characterDb);
            characterMemory.Load();
            var gameMain = GameMain.CreateGameMain(characterMemory);
            Router.Of(node).Overwrite(gameMain);

        })));
        return node;
    }
}
