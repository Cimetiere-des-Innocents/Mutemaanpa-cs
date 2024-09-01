namespace Mutemaanpa;

using Godot;
using Kernel;

public partial class LoadGame : ScrollContainer
{
    [Export]
    VBoxContainer? saveList;

    Game? Game;

    [Export]
    Button? backButton;

    public static LoadGame CreateLoadGame(Game game)
    {
        var node = ResourceLoader.Load<PackedScene>("res://scene/tool/load/load_game.tscn")
            .Instantiate<LoadGame>();
        node.Game = game;
        node.backButton!.Pressed += Router.Of(node).Pop;
        return node;
    }

    SaveSlot ToSaveSlot(SaveData save) => SaveSlot.CreateSaveSlot(save,
        loadGameBehavior: () =>
        {
            var gameMain = GameMain.CreateGameMain(Game!.loadSession(save.Id), Game!.saveSession);
            Router.Of(this).Overwrite(gameMain);
        },
        deleteSaveBehavior: () =>
        {
            Game!.remove(save.Id);
            Sync();
        });

    private void Sync()
    {
        foreach (var c in saveList!.GetChildren())
        {
            saveList.RemoveChild(c);
            c.QueueFree();
        }
        foreach (var save in Game!.catalog.querySaves())
        {
            saveList.AddChild(ToSaveSlot(save));
        }
    }
}
