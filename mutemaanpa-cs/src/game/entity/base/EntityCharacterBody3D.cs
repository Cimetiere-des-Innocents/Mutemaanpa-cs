using System;
using Godot;

namespace Mutemaanpa;

public partial class EntityCharacterBody3D : CharacterBody3D, Entity<CharacterBody3D>
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

	public override void _Ready()
	{
		type = EntityTypeUtil.Reflect(this);
		DefineData(new EntityDataBuilder(this));
	}

	public virtual void Load(SaveDict data)
	{
		Transform = SaveUtil.LoadTransform((SaveList)data["transform"]);
		EntityUtil.LoadCustomData(data, this);
	}

	public virtual void Save(SaveDict data)
	{
		data["transform"] = SaveUtil.SaveTransform(Transform);
		EntityUtil.SaveCustomData(data, this);
	}

	public virtual void DefineData(EntityDataBuilder builder) { }
}
