using Godot;

namespace Mutemaanpa;
public partial class Player : Character
{
    private double lastDamaged = 0.0;

    public override void _PhysicsProcess(double delta)
    {
        var input2d = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
        var input = new Vector3(input2d.X, 0, input2d.Y);

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

        Velocity = GetVelocity(input);
        MoveAndSlide();
    }
}
