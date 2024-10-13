using Godot;

namespace Mutemaanpa;

[Tool]
public partial class TerrainPreview : MeshInstance3D
{
    public required int ChunkX;
    public required int ChunkZ;

    public override void _Ready()
    {
        ArrayMesh mesh;
        ConcavePolygonShape3D? shape;
        float yOffset;
        TerrainGenerator.Generate(ChunkX, ChunkZ, false, out mesh, out shape, out yOffset);
        var heightOffset = GetParent<Chunk>().HeightOffset;
        Position += new Vector3(0.0f, yOffset - heightOffset, 0.0f);
        Mesh = mesh;
        MaterialOverride = TerrainMaterialHolder.Get();
    }
}
