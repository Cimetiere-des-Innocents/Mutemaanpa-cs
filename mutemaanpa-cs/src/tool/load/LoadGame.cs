namespace Mutemaanpa;

using System;
using System.Collections.Generic;
using System.IO;
using Dapper;
using Godot;

public partial class LoadGame : ScrollContainer
{
    [Export]
    VBoxContainer? saveList;

    readonly Dictionary<Guid, Node> saveIdToListIdx = [];

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
            .ForEach((save) =>
            {
                var saveFile = $"m8a_save_{save.Id}.db";
                var child = SaveSlot.CreateSaveSlot(save, () =>
                {
                    var characterDb = new CharacterDatabase($"Data Source={saveFile}");
                    var characterMemory = new CharacterMemory(characterDb);
                    characterMemory.Load();
                    var gameMain = GameMain.CreateGameMain(characterMemory);
                    Router.Of(node).Overwrite(gameMain);

                }, () =>
                {
                    saveDatabase.Remove(save.Id);
                    node.saveList!.RemoveChild(node.saveIdToListIdx.TryGetValue(save.Id, out var value)
                        ? value
                        : throw new Exception($"Save {save.Id} not exist."));
                    node.saveIdToListIdx.Remove(save.Id);
                    File.Delete(saveFile);
                });
                node.saveList!.AddChild(child);
                node.saveIdToListIdx.Add(save.Id, child);
            });
        return node;
    }
}
