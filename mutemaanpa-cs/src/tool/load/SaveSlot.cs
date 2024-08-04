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

    [Export]
    Button? DeleteSave;

    public static SaveSlot CreateSaveSlot(SaveData saveData,
                                          Action loadGameBehavior,
                                          Action deleteSaveBehavior)
    {
        var node = ResourceLoader.Load<PackedScene>("res://scene/tool/load/save_slot.tscn")
            .Instantiate<SaveSlot>();
        node.SaveName!.Text = saveData.Id.ToString();
        node.SaveTime!.Text = saveData.LastPlayed.ToString();
        node.LoadGame!.Pressed += loadGameBehavior;
        node.DeleteSave!.Pressed += deleteSaveBehavior;
        return node;
    }
}
