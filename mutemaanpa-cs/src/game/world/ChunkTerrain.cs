using System.Collections.Generic;
using Godot;

namespace Mutemaanpa;

class GlobalHeightMapHolder
{
    private static float MAX_HEIGHT = 2560.0f;

    private static Texture2D? globalHeightMap;

    private static Texture2D Get()
    {
        if (globalHeightMap != null)
        {
            return globalHeightMap;
        }

        globalHeightMap = GD.Load<Texture2D>("res://asset/image/global_heightmap.png");
        return globalHeightMap;
    }

    public static float Sample(int chunkX, int chunkZ, int x, int z)
    {
        var hMap = Get();
        int i = chunkX * 4 + x;
        int j = chunkZ * 4 + z;
        if (i < 0 || i >= 1024 || j < 0 || j >= 1024)
        {
            return 0.0f;
        }

        return hMap.GetImage().GetPixel(i, j).R * MAX_HEIGHT;
    }
}

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
        float[,] heights = new float[6, 6];
        for (int i = -1; i < 5; i++)
        {
            for (int j = -1; j < 5; j++)
            {
                heights[i + 1, j + 1] = GlobalHeightMapHolder.Sample(ChunkX, ChunkZ, i, j);
            }
        }

        float minVertexHeight = float.MaxValue;
        float[,] vertexHeights = new float[5, 5];
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                var temp = (heights[i, j] + heights[i, j + 1] + heights[i + 1, j] + heights[i + 1, j + 1]) / 4;
                if (temp < minVertexHeight)
                {
                    minVertexHeight = temp;
                }
                vertexHeights[i, j] = temp;
            }
        }
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                vertexHeights[i, j] -= minVertexHeight;
            }
        }

        var surfaceArray = new Godot.Collections.Array();
        surfaceArray.Resize((int)Mesh.ArrayType.Max);
        var vertices = new List<Vector3>();
        var uvs = new List<Vector2>();
        var normals = new List<Vector3>();
        var indicesMap = new int[5, 5];
        var indices = new List<int>();
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                indicesMap[i, j] = vertices.Count;

                var height = vertexHeights[i, j];
                vertices.Add(new Vector3(i * 32.0f, height, j * 32.0f));
                uvs.Add(new Vector2(i * 0.25f, j * 0.25f));

                var a1 = new Vector3(-16.0f, heights[i, j] - height, -16.0f).Normalized();
                var a2 = new Vector3(16.0f, heights[i + 1, j] - height, -16.0f).Normalized();
                var b1 = new Vector3(-16.0f, heights[i, j + 1] - height, 16.0f).Normalized();
                var b2 = new Vector3(16.0f, heights[i + 1, j + 1] - height, 16.0f).Normalized();
                var n1 = b1.Cross(a1);
                var n2 = b2.Cross(a2);
                normals.Add((n1 + n2).Normalized());
            }
        }

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                indices.Add(indicesMap[i, j]);
                indices.Add(indicesMap[i + 1, j]);
                indices.Add(indicesMap[i + 1, j + 1]);
                indices.Add(indicesMap[i, j]);
                indices.Add(indicesMap[i + 1, j + 1]);
                indices.Add(indicesMap[i, j + 1]);
            }
        }

        surfaceArray[(int)Mesh.ArrayType.Vertex] = vertices.ToArray();
        surfaceArray[(int)Mesh.ArrayType.TexUV] = uvs.ToArray();
        surfaceArray[(int)Mesh.ArrayType.Normal] = normals.ToArray();
        surfaceArray[(int)Mesh.ArrayType.Index] = indices.ToArray();

        var arrMesh = new ArrayMesh();
        arrMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, surfaceArray);
        mesh.Mesh = arrMesh;
        mesh.MaterialOverride = material;

        var polyShape = new ConcavePolygonShape3D();
        var faces = arrMesh.GetFaces();
        polyShape!.SetFaces(faces);
        shape.Shape = polyShape;

        var yOffset = new Vector3(0, minVertexHeight, 0);
        mesh.Position += yOffset;
        shape.Position += yOffset;
    }
}
