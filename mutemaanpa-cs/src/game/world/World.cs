using System.Collections.Generic;
using Godot;

namespace Mutemaanpa;

public partial class World : Node3D
{
    private readonly Dictionary<Vector2I, string> preDefinedChunks = [];

    private readonly Dictionary<Vector2I, Chunk> activeChunks = [];

    public required DirAccess currentSaveDir;

    public void FindPredefinedChunks()
    {
        var chunkDir = DirAccess.Open("res://scene");
        if (!chunkDir.DirExists("world/chunks"))
        {
            return;
        }
        chunkDir.ChangeDir("world/chunks");


        Queue<string> directories = new Queue<string>();
        directories.Enqueue(".");

        while (directories.Count > 0)
        {
            string currentDir = directories.Dequeue();
            var dir = DirAccess.Open($"{chunkDir.GetCurrentDir()}/{currentDir}");

            dir.ListDirBegin();
            string fileName = dir.GetNext();
            while (fileName != "")
            {
                if (dir.CurrentIsDir())
                {
                    directories.Enqueue($"{currentDir}/{fileName}");
                }
                else if (fileName.StartsWith("chunk-") && fileName.EndsWith(".tscn"))
                {
                    string[] parts = fileName.Replace("chunk-", "").Replace(".tscn", "").Split("-");
                    if (parts.Length == 2 && int.TryParse(parts[0], out int x) && int.TryParse(parts[1], out int z))
                    {
                        preDefinedChunks.Add(new Vector2I(x, z), $"res://scene/world/chunks/{currentDir}/{fileName}");
                    }
                }
                fileName = dir.GetNext();
            }
            dir.ListDirEnd();
        }
    }

    public void SpawnChunk(int x, int z)
    {
        var v2i = new Vector2I(x, z);
        Chunk chunk;
        if (preDefinedChunks.ContainsKey(v2i))
        {
            chunk = GD.Load<PackedScene>(preDefinedChunks[v2i]).Instantiate<Chunk>();
        }
        else
        {
            chunk = new Chunk()
            {
                ChunkX = x,
                ChunkZ = z
            };
        }
        AddChild(chunk);
        activeChunks.Add(v2i, chunk);

        chunk.Load(currentSaveDir);
        chunk.SpawnAllEntities();
    }

    public void DestroyChunk(int x, int z)
    {
        var v2i = new Vector2I(x, z);
        if (activeChunks.ContainsKey(v2i))
        {
            var chunk = activeChunks[v2i];
            activeChunks.Remove(v2i);

            chunk.Save(currentSaveDir);
        }
    }
}