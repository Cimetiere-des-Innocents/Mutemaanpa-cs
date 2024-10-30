using Godot;

namespace Mutemaanpa;

public class TerrainMaterialHolder
{
    static Material? material;
    public static Material Get()
    {
        if (material is null)
        {
            material = GD.Load<Material>("res://asset/material/terrain.tres");
        }
        return material;
    }
}

public partial class ChunkTerrain : StaticBody3D
{
#pragma warning disable CS8618
    [Export]
    private MeshInstance3D mesh;

    [Export]
    private CollisionShape3D shape;
#pragma warning restore CS8618

    public int ChunkX = 0;
    public int ChunkZ = 0;

    public override void _Ready()
    {
        ArrayMesh arrMesh;
        ConcavePolygonShape3D? polyShape;
        float yOffset;
        TerrainGenerator.Generate(ChunkX, ChunkZ, true, out arrMesh, out polyShape, out yOffset);

        mesh.Mesh = arrMesh;
        mesh.MaterialOverride = TerrainMaterialHolder.Get();
        shape.Shape = polyShape;

        var offset = new Vector3(0, yOffset, 0);
        mesh.Position += offset;
        shape.Position += offset;
    }
}
