namespace Mutemaanpa;

using System;
using Godot;

public partial class SaveSlot : PanelContainer
{
    [Export]
    Label? SaveName;

    [Export]
    Label? SaveTime;

    [Export]
    Button? LoadGame;

    public static SaveSlot CreateSaveSlot(SaveData saveData)
    {
        var node = ResourceLoader.Load<PackedScene>("res://scene/tool/load/save_slot.tscn")
            .Instantiate<SaveSlot>();
        node.SaveName!.Text = saveData.Id.ToString();
        node.SaveTime!.Text = saveData.LastPlayed.ToString();
        node.LoadGame!.Pressed += () =>
        {
            var characterDb = new CharacterDatabase($"Data Source=m8a_save_{saveData.Id}.db");
            var characterManager = new CharacterManager(characterDb);
            var gameMain = GameMain.CreateGameMain(characterManager);
            Router.Of(node).Overwrite(gameMain);

        };
        return node;
    }

}
