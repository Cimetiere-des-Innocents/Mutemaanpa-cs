using Godot;

namespace Mutemaanpa;

[Tool]
public partial class EntitySpawner : Node3D
{
    [Export]
    private PackedScene? packedScene;

    public virtual T? SpawnEntity<T>(bool asChild = false, bool freeThis = true) where T : class, Entity<Node3D>
    {
        var entity = packedScene?.Instantiate<Node3D>();

        if (entity == null)
        {
            Logger.Warn("Entity spawner spawned null");
        }

        if (asChild)
        {
            AddChild(entity);
        }
        else
        {
            GetParent().AddChild(entity);
        }

        entity?.AddToGroup("entity");

        if (!Engine.IsEditorHint() && entity is Entity<Node3D> realEntity)
        {
            initEntity(realEntity);
            realEntity.OnSpawned();
        }

        if (!asChild && freeThis)
        {
            QueueFree();
        }

        if (entity is T rightTypeEntity)
        {
            return rightTypeEntity;
        }
        Logger.Warn("Entity Spawner spawned wrong type");
        return null;
    }

    protected virtual void initEntity(Entity<Node3D> entity) { }

    public override void _Ready()
    {
        if (Engine.IsEditorHint())
        {
            SpawnEntity<Entity<Node3D>>(true, false);
        }
    }
}
