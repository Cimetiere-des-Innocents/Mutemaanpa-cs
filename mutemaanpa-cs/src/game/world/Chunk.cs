using System;
using System.Collections.Generic;
using Godot;

namespace Mutemaanpa;

public partial class Chunk : Node3D
{
    [Export]
    public required int ChunkX;

    [Export]
    public required int ChunkZ;

    private readonly Dictionary<Guid, Entity<Node3D>> entities = [];

    private readonly Dictionary<string, bool> spawned = [];

    public bool HasSpawned(ChunkEntitySpawner spawner)
    {
        if (!spawned.ContainsKey(spawner.Name))
        {
            return false;
        }
        return spawned[spawner.Name];
    }

    public void AddEntity(Entity<Node3D> entity)
    {
        entities[entity.uuid] = entity;
    }

    public void Save(DirAccess baseDir)
    {
        var savedEntities = new SaveList();
        foreach (var i in entities)
        {
            savedEntities.Add(i.Key.ToString());
        }

        var savedSpawned = new SaveList();
        foreach (var i in spawned)
        {
            if (i.Value)
            {
                savedSpawned.Add(i.Key);
            }
        }
        var savedChunk = new SaveDict() {
            { "saved_entities", savedEntities },
            { "saved_spawned", savedSpawned }
        };
        using var file = FileAccess.Open($"{baseDir.GetCurrentDir()}/chunk-{ChunkX}-{ChunkZ}.json", FileAccess.ModeFlags.Write);
        file.StoreString(Json.Stringify(savedChunk));
        foreach (var entity in entities)
        {
            using var entityFile = FileAccess.Open($"{baseDir.GetCurrentDir()}/entity-{entity.Key}.json", FileAccess.ModeFlags.Write);
            var entitySaveDict = new SaveDict();
            entity.Value.Save(entitySaveDict);
            entityFile.StoreString(Json.Stringify(entitySaveDict));
        }
    }

    public void Load(DirAccess baseDir)
    {
        if (baseDir.FileExists($"chunk-{ChunkX}-{ChunkZ}.json"))
        {
            using var file = FileAccess.Open($"{baseDir.GetCurrentDir()}/chunk-{ChunkX}-{ChunkZ}.json", FileAccess.ModeFlags.Write);
            var savedChunk = Json.ParseString(file.GetAsText()).As<SaveDict>();
            var savedEntities = savedChunk["saved_entities"].As<SaveList>();
            var savedSpawned = savedChunk["saved_spawned"].As<SaveList>();

            foreach (var i in savedEntities)
            {
                var uuid = Guid.Parse(i.As<string>());
                AddChild
                (
                    new SavedEntitySpawner()
                    {
                        Uuid = uuid,
                        SaveDir = baseDir
                    }
                );
            }

            foreach (var i in savedSpawned)
            {
                spawned[i.As<string>()] = true;
            }
        }
    }

    public void SpawnAllEntities()
    {
        foreach (var node in GetChildren())
        {
            if (node is EntitySpawner entitySpawner)
            {
                entitySpawner.SpawnEntity<Entity<Node3D>>();
            }
        }
    }
}