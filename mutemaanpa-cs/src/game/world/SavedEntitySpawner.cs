using System;
using Godot;

namespace Mutemaanpa;

public partial class SavedEntitySpawner : EntitySpawner
{
    public required Guid Uuid;
    public required DirAccess SaveDir;

    protected override void initEntity(Entity<Node3D> entity)
    {
        if (SaveDir.FileExists($"entity-{Uuid}.json"))
        {
            using var file = FileAccess.Open($"{SaveDir.GetCurrentDir()}/entity-{Uuid}.json", FileAccess.ModeFlags.Read);
            var dict = Json.ParseString(file.GetAsText()).As<SaveDict>();
            entity.Load(dict);
        }
    }
}