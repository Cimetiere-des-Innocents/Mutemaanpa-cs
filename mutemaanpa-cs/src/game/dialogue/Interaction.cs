namespace Mutemaanpa;

using Dapper;
using Godot;
using System.Linq;

/// <summary>
/// Interaction adds interactive functionality to PhysicsBody3D.
/// 
/// REQUIRES: its parent node is PhysicsBody3D and has Head(Label3D) and Mesh 
/// </summary>
public partial class Interaction : Node3D
{
    ShaderMaterial? outline;
    Label3D? banterBox;
    Journal? journal;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        RegisterShader();
        RegisterOutlineHandler();
        RegisterBanterBox();
        SetClickHandler();
        journal = Journal.Of(this);
    }

    private void RegisterOutlineHandler()
    {
        var body = GetParent<PhysicsBody3D>();
        body.InputRayPickable = true;
        body.MouseEntered += ShowOutline;
        body.MouseExited += HideOutline;
        HideOutline();
    }

    private void RegisterShader()
    {
        var body = GetParent<PhysicsBody3D>();
        var meshes = (from c in body.GetChildren() where c is MeshInstance3D select c as MeshInstance3D).AsList();
        if (meshes.Count == 0)
        {
            return;
        }
        var outlineShader = ResourceLoader.Load<Shader>("res://asset/shader/interaction_border.gdshader");
        outline = new ShaderMaterial
        {
            Shader = outlineShader,
            RenderPriority = 0
        };
        var mesh = meshes.First();
        var material = mesh.GetActiveMaterial(0);
        if (material is null)
        {
            material = new StandardMaterial3D();
            mesh.SetSurfaceOverrideMaterial(0, material);
        }
        while (material._CanDoNextPass() && material.NextPass is ShaderMaterial shaderMaterial)
        {
            material = shaderMaterial;
        }
        material.NextPass = outline;
    }

    public void HideOutline()
    {
        outline?.SetShaderParameter("cutoff", 1e10f);
    }

    public void ShowOutline()
    {
        outline?.SetShaderParameter("cutoff", 0.1f);
    }

    private void RegisterBanterBox()
    {
        banterBox = GetNode<Label3D>("../Head");
        banterBox.Text = "And you thought rivellon was flat.";
        banterBox!.Hide();
    }

    private void SetClickHandler()
    {
        var body = GetParent<PhysicsBody3D>();
        body.Connect("input_event", Callable.From<Node, InputEvent, Vector3, Vector3, long>(ConnectClickEvents));
    }

    protected virtual void ConnectClickEvents(Node camera, InputEvent @event, Vector3 position, Vector3 normal, long shape_idx)
    {
        if (@event is InputEventMouseButton inputEventMouseButton
        && inputEventMouseButton.IsReleased()
        && inputEventMouseButton.ButtonIndex == MouseButton.Left)
        {
            banterBox!.Show();
            var timer = GetTree().CreateTimer(3.0);
            timer.Timeout += banterBox!.Hide;
        }
    }
}


