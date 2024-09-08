using System;
using Godot;

namespace Mutemaanpa;

public abstract partial class EntityCharacterBody3D : CharacterBody3D, Entity<CharacterBody3D>
{
    private IEntityType<Entity<Node3D>>? type;

    public IEntityType<Entity<Node3D>> Type
    {
        get
        {
            if (type == null)
            {
                throw new Exception($"Trying to access entity type of class {GetType().Name} before _Ready is called");
            }
            return type;
        }
    }

    private readonly EntityDataMap dataMap = new();

    public EntityDataMap DataMap => dataMap;

    public CharacterBody3D Value => this;

    public Guid uuid => EntityUtil.UUID[this];

    protected Vector3 gravity;

    public override void _Ready()
    {
        type = EntityTypeUtil.Reflect(this);
        DefineData(new EntityDataBuilder(this));
        gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity") * (Vector3)ProjectSettings.GetSetting("physics/3d/default_gravity_vector");
    }

    public virtual void DefineData(EntityDataBuilder builder)
    {
        builder.define(GodotDataKeys.TRANSFORM);
        builder.define(EntityUtil.UUID, Guid.NewGuid());
    }

    public override void _PhysicsProcess(double delta)
    {
        if (GetParent() is Chunk)
        {
            Chunk.ProcessEntity(this);
        }
    }

    public virtual void OnSpawned() { }

    public virtual void OnChunkTick(Chunk chunk) { }
}
