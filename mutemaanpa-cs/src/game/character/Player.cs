using System.Linq;
using Godot;

namespace Mutemaanpa;

[Entity]
public partial class Player : Character
{
    public static readonly EntityType<Player> TYPE = EntityTypeUtil.FromScene<Player>("player", "res://scene/game/character/Player.tscn");

    private double lastDamaged = 0.0;

    private Vector2 lastXZ = Vector2.Zero;

    public override void Tick(double delta)
    {
        var input2d = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
        var input = new Vector3(input2d.X, Velocity.Y, input2d.Y);

        foreach (var kinematicCollision3D in this.GetSlideCollisions())
        {
            if (kinematicCollision3D.GetCollider() is RigidBody3D rigidBody && rigidBody.Name == "Unnis")
            {
                lastDamaged += delta;
                if (lastDamaged > 0.5)
                {
                    Hit(1.0f);
                    lastDamaged -= 0.5;
                }
            }
        }

        var velocity = GetVelocity(input);
        velocity += gravity * (float)delta;
        Velocity = velocity;
        MoveAndSlide();

        var head = GetChildren().Where((node) => node is Label3D).First() as Label3D;
        var y = GlobalPosition.Y.ToString("0.0000");
        head!.Text = $"y: {y}";
    }

    public override void OnSpawned()
    {
        var parent = GetParent();
        if (parent != null)
        {
            var parentParent = parent.GetParent();
            if (parentParent is World world)
            {
                world.Player = this;
            }
        }
    }

    public override void OnChunkTick(Chunk chunk)
    {
        var world = chunk.World;
        if (Mathf.Abs(Position.X - lastXZ.X) >= 64 || Mathf.Abs(Position.Z - lastXZ.Y) >= 64)
        {
            world.UpdateChunks(Position.X, Position.Z);
            lastXZ = new Vector2(Position.X, Position.Z);
        }
    }
}
