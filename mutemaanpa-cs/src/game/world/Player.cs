namespace Mutemaanpa;

using Godot;

public partial class Player : CharacterBody3D
{
    private Character? player;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        player = GetNode<GameMain>("../../..").characterMemory!.GetPlayer();
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

        Position = player!.Move(input, (float)delta);
    }
}
