using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Mutemaanpa;

public record struct SaveData(
    Guid Id,
    DateTime CreatedAt,
    DateTime LastPlayed
);

public partial class Catalog : Node
{
    public static readonly string m8aDir = "m8a/";
    public static readonly string path = m8aDir + "mutamaanpa.json";

    public List<SaveData> Saves { get; private set; } = [];

    public override void _Ready()
    {
        Load();
    }

    public void Load()
    {
        if (!FileAccess.FileExists(path))
        {
            return;
        }
        using var catalogFile = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        var data = catalogFile.GetAsText();
        Saves = System.Text.Json.JsonSerializer.Deserialize<List<SaveData>>(data)!;
    }

    public static DirAccess ToGameFS()
    {
        var dir = DirAccess.Open(".");
        if (!dir.DirExists(m8aDir))
        {
            dir.MakeDir("m8a");
        }
        dir.ChangeDir("m8a");
        return dir;
    }

    public void Save()
    {
        using var catalogFile = FileAccess.Open(path, FileAccess.ModeFlags.Write);
        catalogFile.StoreString(System.Text.Json.JsonSerializer.Serialize(Saves));
    }

    public Guid NewSave()
    {
        Guid saveUuid = Guid.NewGuid();
        Saves.Add(new SaveData(saveUuid, DateTime.Now, DateTime.Now));
        return saveUuid;
    }

    internal void Remove(Guid id)
    {
        Saves = Saves.Where(save => save.Id != id).ToList();
        Save();
        var dir = ToGameFS();
        var saveDir = $"m8a_{id}";
        dir.ChangeDir(saveDir);
        foreach (var f in dir.GetFiles())
        {
            dir.Remove(f);
        }
        dir.ChangeDir("..");
        dir.Remove(saveDir);
    }
}
