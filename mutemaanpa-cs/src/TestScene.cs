using Godot;

namespace Mutemaanpa;

public partial class TestScene : Node3D
{
    [Export]
    private EntitySpawner? playerSpawner;

    private Player? player;

    public static DirAccess ToGameFS()
    {
        var dir = DirAccess.Open(".");
        if (!dir.DirExists("m8a"))
        {
            dir.MakeDir("m8a");
        }
        dir.ChangeDir("m8a");
        return dir;
    }

    public void SpawnPlayer()
    {
        player = playerSpawner?.SpawnEntity<Player>();
        var dir = Main.Get(this).Pwd()!;
        if (dir.FileExists("testSave.json"))
        {
            using var file = FileAccess.Open($"{dir.GetCurrentDir()}/testSave.json", FileAccess.ModeFlags.Read);
            var content = file.GetAsText();
            var parseResult = Json.ParseString(content);
            (player! as Entity<Node3D>).Load(parseResult.As<SaveDict>());
        }
    }

    public void SavePlayer()
    {
        if (player != null)
        {
            var dir = Main.Get(this).Pwd()!;
            using var file = FileAccess.Open($"{dir.GetCurrentDir()}/testSave.json", FileAccess.ModeFlags.Write);
            var saveDict = new SaveDict();
            (player as Entity<Node3D>).Save(saveDict);
            var saveJson = Json.Stringify(saveDict);
            file.StoreString(saveJson);
        }
    }

    public void QuitGame()
    {
        GetTree().Quit();
    }
}
