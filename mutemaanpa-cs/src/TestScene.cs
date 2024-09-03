using Godot;

namespace Mutemaanpa;

public partial class TestScene : Node3D
{
    [Export]
    private EntitySpawner? playerSpawner;

    public void SpawnPlayer()
    {
        var player = playerSpawner?.SpawnEntity<Player>();
    }

    public void QuitGame()
    {
        GetTree().Quit();
    }
}
