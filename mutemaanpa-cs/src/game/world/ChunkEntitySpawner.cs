using System;
using Godot;

namespace Mutemaanpa;

[Tool]
public partial class ChunkEntitySpawner : EntitySpawner
{
    public Chunk Chunk
    {
        get
        {
            var parent = GetParent();
            if (parent is Chunk chunk)
            {
                return chunk;
            }
            throw new Exception("ChunkEntitySpawner not in chunk");
        }
    }

    public override string[] _GetConfigurationWarnings()
    {
        var parent = GetParent();
        if (parent is not Mutemaanpa.Chunk)
        {
            return ["ChunkEntitySpawner must be placed in a chunk"];
        }

        foreach (var node in Chunk.GetChildren())
        {
            if (node is ChunkEntitySpawner spawner && node != this && spawner.Name == Name)
            {
                return ["Duplicate ChunkEntitySpawner names in one chunk"];
            }
        }

        return [];
    }

    public override T? SpawnEntity<T>(bool asChild = false, bool freeThis = true) where T : class
    {
        if (Engine.IsEditorHint())
        {
            return base.SpawnEntity<T>(asChild, freeThis);
        }

        if (Chunk.HasSpawned(this))
        {
            return null;
        }

        return base.SpawnEntity<T>(asChild, freeThis);
    }
}
