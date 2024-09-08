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


        void slotHandler(SaveData save)
        {
            void loadGameBehavior()
            {
                var testScene = ResourceLoader.Load<PackedScene>("res://scene/world/world.tscn")
                    .Instantiate<Node3D>();
                catalog.UseGame(save.Id);
                Router.Of(this).Overwrite(testScene);
            }

            void deleteSaveBehavior()
            {
                catalog.Remove(save.Id);
                Sync();
            }

            var saveFile = $"m8a_save_{save.Id}.db";
            var child = SaveSlot.CreateSaveSlot(
                save,
                loadGameBehavior,
                deleteSaveBehavior
            );
            saveList!.AddChild(child);
        }

        catalog!.Saves.ForEach(slotHandler);
    }
}
