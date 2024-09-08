using System;
using Godot;
using Mutemaanpa;

[Tool]
public partial class ChunkEntitySpawner : EntitySpawner
{
    [Export]
    public string SpawnerName = "";

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

        var entity = base.SpawnEntity<T>(asChild, freeThis);
        if (!Engine.IsEditorHint() && entity != null)
        {
            Chunk.AddEntity(entity);
            entity.Value.Position = new Vector3()
            {
                X = Position.X + (Chunk.ChunkX - 128) * 128,
                Y = Position.Y,
                Z = Position.Z + (Chunk.ChunkZ - 128) * 128
            };
        }
        return entity;
    }
}
