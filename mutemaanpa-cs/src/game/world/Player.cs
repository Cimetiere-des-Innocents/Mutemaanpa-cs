namespace Mutemaanpa;

using Godot;

public partial class Player : CharacterBody3D
{
    private Character? player;
    private double lastDamaged = 0.0;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        player = GetNode<GameMain>("../../..").CharacterMemory!.GetPlayer();
    }

    public override void _PhysicsProcess(double delta)
    {
        var input = new Vector3();
        if (Input.IsActionPressed("move_forward"))
        {
            input += Vector3.Forward;
        }
        if (Input.IsActionPressed("move_backward"))
        {
            input += Vector3.Back;
        }
        if (Input.IsActionPressed("move_left"))
        {
            input += Vector3.Left;
        }
        if (Input.IsActionPressed("move_right"))
        {
            input += Vector3.Right;
        }

        foreach (var kinematicCollision3D in this.GetSlideCollisions())
        {
            if (kinematicCollision3D.GetCollider() is RigidBody3D rigidBody && rigidBody.Name == "DeadZone")
            {
                lastDamaged += delta;
                if (lastDamaged > 0.5)
                {
                    player!.Hit(1.0f);
                    lastDamaged -= 0.5;
                }
            }
        }

        Velocity = player!.GetVelocity(input);
        MoveAndSlide();
    }
}
