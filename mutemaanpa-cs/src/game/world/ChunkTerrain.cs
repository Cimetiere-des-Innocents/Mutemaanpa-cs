using System.Collections.Generic;
using Godot;

namespace Mutemaanpa;

public partial class ChunkTerrain : StaticBody3D
{
#pragma warning disable CS8618
    [Export]
    private MeshInstance3D mesh;

    [Export]
    private CollisionShape3D shape;

    [Export]
    private StandardMaterial3D material;
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
        mesh.MaterialOverride = material;
        shape.Shape = polyShape;

        var offset = new Vector3(0, yOffset, 0);
        mesh.Position += offset;
        shape.Position += offset;
    }
}
