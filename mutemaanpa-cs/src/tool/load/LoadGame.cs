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
            .ForEach((save) => node.saveList!.AddChild(SaveSlot.CreateSaveSlot(save)));
        return node;
    }
}
