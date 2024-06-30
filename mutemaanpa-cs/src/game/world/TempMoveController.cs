namespace Mutemaanpa;

using Godot;

public partial class TempMoveController : Node3D
{
    [Export]
    private float speed = 5.0f;
    private Node3D? player;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        player = GetParent<Node3D>();
    }

    public override void _PhysicsProcess(double delta)
    {
        var velocity = new Vector3();

        if (Input.IsActionPressed("move_forward"))
        {
            velocity += Vector3.Forward;
        }
        if (Input.IsActionPressed("move_backward"))
        {
            velocity += Vector3.Back;
        }
        if (Input.IsActionPressed("move_left"))
        {
            velocity += Vector3.Left;
        }
        if (Input.IsActionPressed("move_right"))
        {
            velocity += Vector3.Right;
        }

        velocity = velocity.Normalized() * speed * (float)delta;

        player!.Position += velocity;
    }
}
