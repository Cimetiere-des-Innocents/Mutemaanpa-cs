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

/// <summary>
/// Catalog manages the metadata about all game sessions. It tells the game how many saves we have,
/// then give sufficient information for world to bootstrap itself.
/// </summary>
public partial class Catalog : Node
{
    public static readonly string m8aDir = "m8a";
    public static readonly string path = "/mutamaanpa.json";

    public List<SaveData> Saves { get; private set; } = [];
    public Guid? currentSave = null;

    public override void _Ready()
    {
        Load();
    }

    public void Load()
    {
        if (!FileAccess.FileExists(GetCatalogFile()))
        {
            return;
        }
        using var catalogFile = FileAccess.Open(GetCatalogFile(), FileAccess.ModeFlags.Read);
        var data = catalogFile.GetAsText();
        Saves = System.Text.Json.JsonSerializer.Deserialize<List<SaveData>>(data)!;
    }

    private static Catalog Get(Node node)
    {
        return Main.Get(node)!.catalog!;
    }

    /// <summary>
    /// Pwd returns the current user directory.
    /// </summary>
    /// <returns>if the user directory does not exist, returns null.</returns>
    public static DirAccess? Pwd(Node node)
    {
        var dir = Root();
        var saveId = Get(node).currentSave;
        if (saveId == null)
        {
            return dir;
        }
        var saveDir = $"m8a_{saveId}";
        if (!dir.DirExists(saveDir))
        {
            return null;
        }
        dir.ChangeDir(saveDir);
        return dir;
    }

    private static DirAccess Root()
    {
        var dir = DirAccess.Open(".");
        if (!dir.DirExists(m8aDir))
        {
            dir.MakeDir(m8aDir);
        }
        dir.ChangeDir(m8aDir);
        return dir;
    }

    public static DirAccess? PathOfUuid(Guid uuid)
    {
        var dir = Root();
        var saveDir = $"m8a_{uuid}";
        if (!dir.DirExists(saveDir))
        {
            return null;
        }
        dir.ChangeDir(saveDir);
        return dir;
    }

    private static string GetCatalogFile()
    {
        return Root().GetCurrentDir() + path;
    }

    public void SaveCatalog()
    {
        using var catalogFile = FileAccess.Open(GetCatalogFile(), FileAccess.ModeFlags.Write);
        catalogFile.StoreString(System.Text.Json.JsonSerializer.Serialize(Saves));
    }

    public Guid NewSave()
    {
        Guid saveUuid = Guid.NewGuid();
        Logger.Info($"New save {saveUuid} created.");
        Saves.Add(new SaveData(saveUuid, DateTime.Now, DateTime.Now));
        SaveCatalog();
        var root = Root();
        root.MakeDir($"m8a_{saveUuid}");
        return saveUuid;
    }

    internal void Remove(Guid id)
    {
        Saves = Saves.Where(save => save.Id != id).ToList();
        SaveCatalog();
        var dir = PathOfUuid(id);
        if (dir is null)
        {
            return;
        }

        foreach (var f in dir.GetFiles())
        {
            dir.Remove(f);
        }
        var dirToRemove = dir.GetCurrentDir();
        dir.ChangeDir("..");
        dir.Remove(dirToRemove);
    }

    public override void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest)
        {
            SaveCatalog();
        }
    }

    /// <summary>
    /// Tells the catalog to keep track of the data in the save of id
    /// </summary>
    /// <param name="id"></param>
    internal void UseGame(Guid id)
    {
        var save = Saves.Where(save => id == save.Id).First();
        Logger.Info($"playing save {id}.");
        currentSave = save.Id;
    }
}
