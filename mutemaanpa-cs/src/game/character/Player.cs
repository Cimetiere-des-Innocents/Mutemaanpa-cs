using Godot;

namespace Mutemaanpa;

[Entity]
public partial class Player : Character
{
    public static readonly EntityType<Player> TYPE = EntityTypeUtil.FromScene<Player>("player", "res://scene/game/character/player.tscn");

    private double lastDamaged = 0.0;

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

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
    }
}
