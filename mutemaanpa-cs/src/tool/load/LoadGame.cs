using Godot;

namespace Mutemaanpa;
public partial class LoadGame : ScrollContainer
{
    [Export]
    VBoxContainer? saveList;

    Catalog? catalog;

    [Export]
    Button? backButton;

    public static LoadGame CreateLoadGame(Catalog catalog)
    {
        var node = ResourceLoader.Load<PackedScene>("res://scene/tool/load/load_game.tscn")
            .Instantiate<LoadGame>();
        node.backButton!.Pressed += () =>
        {
            Router.Of(node).Pop();
        };
        node.catalog = catalog;
        node.Sync();
        return node;
    }

    private void Sync()
    {
        foreach (var c in saveList!.GetChildren())
        {
            saveList!.RemoveChild(c);
            c.QueueFree();
        }
        catalog!.Saves
            .ForEach((save) =>
            {
                var saveFile = $"m8a_save_{save.Id}.db";
                var child = SaveSlot.CreateSaveSlot(save,
                loadGameBehavior: () =>
                {
                    var gameMain = GameMain.CreateGameMain(save.Id);
                    Router.Of(this).Overwrite(gameMain);
                },
                deleteSaveBehavior: () =>
                {
                    catalog.Remove(save.Id);
                    Sync();
                });
                saveList!.AddChild(child);
            });
    }
}
