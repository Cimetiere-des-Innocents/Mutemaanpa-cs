using System;
using System.Collections.Generic;
using Godot;

namespace Mutemaanpa;

class EscapedEntityInfo
{
    public required Guid Uuid;
    public required int ChunkX;
    public required int ChunkZ;
}

public partial class World : Node3D
{
    private readonly Dictionary<Vector2I, string> preDefinedChunks = [];

    private readonly Dictionary<Vector2I, Chunk> activeChunks = [];

    private readonly List<EscapedEntityInfo> escapedEntities = [];

    public required DirAccess currentSaveDir;

    public Node3D? Player;

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
        SpawnChunk(new Vector2I(x, z));
    }

    public void SpawnChunk(Vector2I pos)
    {
        Chunk chunk;
        if (preDefinedChunks.ContainsKey(pos))
        {
            chunk = GD.Load<PackedScene>(preDefinedChunks[pos]).Instantiate<Chunk>();
        }
        else
        {
            chunk = new Chunk()
            {
                ChunkX = pos.X,
                ChunkZ = pos.Y
            };
        }
        AddChild(chunk);
        activeChunks.Add(pos, chunk);

        foreach (var entity in escapedEntities)
        {
            if (entity.ChunkX == chunk.ChunkX && entity.ChunkZ == chunk.ChunkZ)
            {
                chunk.AddChild(new SavedEntitySpawner()
                {
                    Uuid = entity.Uuid,
                    SaveDir = currentSaveDir
                });
            }
        }

        chunk.Load(currentSaveDir);
        chunk.RandomSpawn();
        chunk.SpawnAllEntities();
    }

    public void DestroyChunk(int x, int z)
    {
        DestroyChunk(new Vector2I(x, z));
    }

    public void DestroyChunk(Vector2I pos)
    {
        if (activeChunks.ContainsKey(pos))
        {
            var chunk = activeChunks[pos];
            activeChunks.Remove(pos);

            chunk.Save(currentSaveDir);
        }
    }

    public Chunk? GetChunk(int x, int z)
    {
        return GetChunk(new Vector2I(x, z));
    }

    public Chunk? GetChunk(Vector2I v2i)
    {
        if (activeChunks.ContainsKey(v2i))
        {
            return activeChunks[v2i];
        }
        return null;
    }

    public static Vector2I ToChunkCoordinate(float x, float z)
    {
        int intX = (int)x + 128 * 128, intZ = (int)z + 128 * 128;
        return new Vector2I(intX / 128, intZ / 128);
    }

    public void MarkEscaped(Entity<Node3D> entity)
    {
        using var entityFile = FileAccess.Open($"{currentSaveDir.GetCurrentDir()}/entity-{entity.uuid}.json", FileAccess.ModeFlags.Write);
        var dict = new SaveDict();
        entity.Save(dict);
        entityFile.StoreString(Json.Stringify(dict));

        var chunkCoordinate = ToChunkCoordinate(entity.Value.Position.X, entity.Value.Position.Z);

        escapedEntities.Add(new EscapedEntityInfo
        {
            Uuid = entity.uuid,
            ChunkX = chunkCoordinate.X,
            ChunkZ = chunkCoordinate.Y
        });

        entity.Value.QueueFree();
    }
}
