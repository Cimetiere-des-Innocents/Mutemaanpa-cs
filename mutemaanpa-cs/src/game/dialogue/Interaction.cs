namespace Mutemaanpa;

using Dapper;
using Godot;
using System;
using System.Linq;

/// <summary>
/// Interaction adds interactive functionality to PhysicsBody3D.
/// </summary>
public partial class Interaction : Node3D
{
    ShaderMaterial? outline;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        RegisterShader();
        RegisterOutlineHandler();
        HideOutline();
    }

    private void RegisterOutlineHandler()
    {
        var body = GetParent<PhysicsBody3D>();
        body.InputRayPickable = true;
        body.MouseEntered += ShowOutline;
        body.MouseExited += HideOutline;
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
}
