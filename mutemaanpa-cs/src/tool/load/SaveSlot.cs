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

    public static SaveSlot CreateSaveSlot(SaveData saveData, Action loadGameBehavior)
    {
        var node = ResourceLoader.Load<PackedScene>("res://scene/tool/load/save_slot.tscn")
            .Instantiate<SaveSlot>();
        node.SaveName!.Text = saveData.Id.ToString();
        node.SaveTime!.Text = saveData.LastPlayed.ToString();
        node.LoadGame!.Pressed += loadGameBehavior;
        return node;
    }
}
