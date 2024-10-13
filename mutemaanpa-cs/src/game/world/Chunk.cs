using System;
using System.Collections.Generic;
using Godot;

namespace Mutemaanpa;

[Tool]
public partial class Chunk : Node3D
{
	[Export]
	public required int ChunkX;

	[Export]
	public required int ChunkZ;

	[Export]
	public int HeightOffset = 0;

	public World World
	{
		get { return GetParent<World>(); }
	}

	public bool IsEdge = false;

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

	public void SetSpawned(ChunkEntitySpawner spawner)
	{
		spawned[spawner.Name] = true;
	}

	public void AddEntity(Entity<Node3D> entity)
	{
		entities[entity.uuid] = entity;
	}

	public void RemoveEntity(Entity<Node3D> entity)
	{
		if (entities.ContainsKey(entity.uuid))
		{
			entities.Remove(entity.uuid);
		}
	}

	public void Save(DirAccess baseDir)
	{
		var savedEntities = new SaveList();
		foreach (var i in entities)
		{
			savedEntities.Add(new SaveDict() {
				{"uuid", i.Key.ToString()},
				{"type", i.Value.Type.Name}
			});
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
			using var file = FileAccess.Open($"{baseDir.GetCurrentDir()}/chunk-{ChunkX}-{ChunkZ}.json", FileAccess.ModeFlags.Read);
			var savedChunk = Json.ParseString(file.GetAsText()).As<SaveDict>();
			var savedEntities = savedChunk["saved_entities"].As<SaveList>();
			var savedSpawned = savedChunk["saved_spawned"].As<SaveList>();

			foreach (var i in savedEntities)
			{
				var dict = i.As<SaveDict>();
				var type = dict["type"].As<string>();
				var uuid = Guid.Parse(dict["uuid"].As<string>());

				AddChild
				(
					new SavedEntitySpawner(type)
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
				var entity = entitySpawner.SpawnEntity<Entity<Node3D>>();
				if (!Engine.IsEditorHint() && entity != null)
				{
					AddEntity(entity);
					if (entitySpawner is ChunkEntitySpawner chunkEntitySpawner)
					{
						SetSpawned(chunkEntitySpawner);
						entity.Value.Position = new Vector3()
						{
							X = entitySpawner.Position.X + (ChunkX - 128) * 128,
							Y = entitySpawner.Position.Y,
							Z = entitySpawner.Position.Z + (ChunkZ - 128) * 128
						};
					}
				}
			}
		}
	}

	public static void ProcessEntity(Entity<Node3D> entity)
	{
		var entityValue = entity.Value;
		var chunk = entityValue.GetParent<Chunk>();
		if (chunk == null)
		{
			return;
		}
		var world = chunk.World;
		var realChunkPos = World.ToChunkCoordinate(entityValue.Position.X, entityValue.Position.Z);
		if (realChunkPos.X != chunk.ChunkX || realChunkPos.Y != chunk.ChunkZ)
		{
			var newChunk = world.GetChunk(realChunkPos);
			if (newChunk == null)
			{
				world.MarkEscaped(entity);
				return;
			}
			chunk.RemoveChild(entity.Value);
			chunk.RemoveEntity(entity);
			newChunk.AddChild(entity.Value);
			newChunk.AddEntity(entity);
		}
		entity.OnChunkTick(entity.Value.GetParent<Chunk>());
	}

	// TODO: real spawn (currently just terrain)
	public void RandomSpawn()
	{
		var terrain = GD.Load<PackedScene>("res://scene/world/chunk_terrain.tscn").Instantiate<ChunkTerrain>();
		terrain.ChunkX = ChunkX;
		terrain.ChunkZ = ChunkZ;
		AddChild(terrain);
		terrain.GlobalPosition = new Vector3()
		{
			X = (ChunkX - 128) * 128,
			Y = 0,
			Z = (ChunkZ - 128) * 128
		};
	}

	public override void _Ready()
	{
		if (Engine.IsEditorHint())
		{
			editorReady();
		}
		else
		{
			gameReady();
		}
	}

	private void gameReady()
	{
		foreach (var child in GetChildren())
		{
			if (child is Node3D node3D)
			{
				node3D.Position += new Vector3(0.0f, HeightOffset, 0.0f);
			}
		}
	}

	private void editorReady()
	{
		var node = new TerrainPreview()
		{
			ChunkX = ChunkX,
			ChunkZ = ChunkZ
		};
		AddChild(node);
	}
}
