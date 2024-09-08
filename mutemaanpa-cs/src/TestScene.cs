using Godot;

namespace Mutemaanpa;

public partial class TestScene : Node3D
{
    [Export]
    private EntitySpawner? playerSpawner;

    [Export]
    private EntitySpawner? unnisSpawner;

    [Export]
    private EntitySpawner? lenaSpawner;

    private Player? player;

    public void SpawnPlayer()
    {
        player = playerSpawner?.SpawnEntity<Player>();
        unnisSpawner?.SpawnEntity<Entity<Node3D>>();
        lenaSpawner?.SpawnEntity<Entity<Node3D>>();
        var dir = Catalog.Pwd(this)!;
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
            var dir = Catalog.Pwd(this)!;
            using var file = FileAccess.Open($"{dir.GetCurrentDir()}/testSave.json", FileAccess.ModeFlags.Write);
            var saveDict = new SaveDict();
            (player as Entity<Node3D>).Save(saveDict);
            var saveJson = Json.Stringify(saveDict);
            file.StoreString(saveJson);
        }
    }

    public void QuitGame()
    {
        Router.Of(this).Pop();
    }
}
