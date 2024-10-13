using System;
using System.Collections.Generic;
using Godot;

namespace Mutemaanpa;

class EscapedEntityInfo
{
	public required string Type;
	public required Guid Uuid;
	public required int ChunkX;
	public required int ChunkZ;

	public override int GetHashCode()
	{
		return Uuid.GetHashCode();
	}

	public override bool Equals(object? obj)
	{
		return obj is EscapedEntityInfo escapedEntityInfo && Uuid == escapedEntityInfo.Uuid;
	}
}

public partial class World : Node3D
{
	[Export]
	private int initialChunkX = 121;

	[Export]
	private int initialChunkZ = 124;

	private readonly Dictionary<Vector2I, string> preDefinedChunks = [];

	private readonly Dictionary<Vector2I, Chunk> activeChunks = [];

	private readonly HashSet<EscapedEntityInfo> escapedEntities = [];

	public required DirAccess currentSaveDir;

	public Node3D? Player;

	public bool Paused = false;

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

	public Chunk SpawnChunk(int x, int z)
	{
		return SpawnChunk(new Vector2I(x, z));
	}

	public Chunk SpawnChunk(Vector2I pos)
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

		var capturedEntities = new HashSet<EscapedEntityInfo>();

		foreach (var entity in escapedEntities)
		{
			if (entity.ChunkX == chunk.ChunkX && entity.ChunkZ == chunk.ChunkZ)
			{
				chunk.AddChild(new SavedEntitySpawner(entity.Type)
				{
					Uuid = entity.Uuid,
					SaveDir = currentSaveDir
				});
				capturedEntities.Add(entity);
			}
		}

		foreach (var entity in capturedEntities)
		{
			escapedEntities.Remove(entity);
		}

		chunk.Load(currentSaveDir);
		chunk.RandomSpawn();
		chunk.SpawnAllEntities();
		return chunk;
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
			chunk.QueueFree();
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

		var chunk = entity.Value.GetParent<Chunk>();
		chunk!.RemoveEntity(entity);
		chunk!.RemoveChild(entity.Value);

		escapedEntities.Add(new EscapedEntityInfo
		{
			Type = entity.Type.Name,
			Uuid = entity.uuid,
			ChunkX = chunkCoordinate.X,
			ChunkZ = chunkCoordinate.Y
		});

		entity.Value.QueueFree();
	}

	public override void _Ready()
	{
		FindPredefinedChunks();
		currentSaveDir = Catalog.Pwd(this)!;
		if (currentSaveDir.FileExists("world.json"))
		{
			Load();
			return;
		}
		UpdateChunks(new Vector2I(initialChunkX, initialChunkZ));
	}

	public override void _EnterTree()
	{
		var musicPlayer = MusicPlayer.Of(this);
		if (musicPlayer.Status != "World")
		{
			musicPlayer.Status = "World";
			musicPlayer.StopMusic();
			musicPlayer.Stream = GD.Load<AudioStream>("res://asset/music/globulin.ogg");
			musicPlayer.PlayDelayed(1.5);
		}
	}

	public void UpdateChunks(float x, float z)
	{
		UpdateChunks(ToChunkCoordinate(x, z));
	}

	public void UpdateChunks(Vector2I chunkCoord)
	{
		var chunksToRemove = new List<Vector2I>();
		foreach (var pair in activeChunks)
		{
			var pos = pair.Key;
			if (Math.Abs(pos.X - chunkCoord.X) > 2 || Math.Abs(pos.Y - chunkCoord.Y) > 2)
			{
				chunksToRemove.Add(pos);
			}
		}
		foreach (var pos in chunksToRemove)
		{
			DestroyChunk(pos);
		}
		for (int i = -2; i <= 2; i++)
		{
			for (int j = -2; j <= 2; j++)
			{
				var pos = new Vector2I(chunkCoord.X + i, chunkCoord.Y + j);
				if (!activeChunks.ContainsKey(pos))
				{
					SpawnChunk(pos);
				}

				activeChunks[pos].IsEdge = i == 2 || j == 2 || i == -2 || j == -2;
			}
		}
	}

	public void Save()
	{
		foreach (var chunk in activeChunks)
		{
			chunk.Value.Save(currentSaveDir);
		}

		var playerChunkCoords = ToChunkCoordinate(Player!.Position.X, Player!.Position.Z);
		var saveDict = new SaveDict()
		{
			{"playerChunkX", playerChunkCoords.X},
			{"playerChunkZ", playerChunkCoords.Y}
		};
		var saveEscaped = new SaveList();
		foreach (var escaped in escapedEntities)
		{
			saveEscaped.Add(new SaveDict() {
				{"type", escaped.Type},
				{"uuid", EntityDataSerializers.UUID.Serialize(escaped.Uuid)},
				{"chunkX", escaped.ChunkX},
				{"chunkZ", escaped.ChunkZ}
			});
		}
		saveDict["escaped_entities"] = saveEscaped;
		using var file = FileAccess.Open($"{currentSaveDir.GetCurrentDir()}/world.json", FileAccess.ModeFlags.Write);
		file.StoreString(Json.Stringify(saveDict));
	}

	public void Load()
	{
		using var file = FileAccess.Open($"{currentSaveDir.GetCurrentDir()}/world.json", FileAccess.ModeFlags.Read);
		var saveDict = Json.ParseString(file.GetAsText()).As<SaveDict>();
		var playerChunkX = saveDict["playerChunkX"].As<int>();
		var playerChunkZ = saveDict["playerChunkZ"].As<int>();
		var saveEscaped = saveDict["escaped_entities"].As<SaveList>();
		foreach (var i in saveEscaped)
		{
			var escaped = i.As<SaveDict>();
			escapedEntities.Add(new EscapedEntityInfo()
			{
				Type = escaped["type"].As<string>(),
				Uuid = EntityDataSerializers.UUID.Deserialize(escaped["uuid"]),
				ChunkX = escaped["chunkX"].As<int>(),
				ChunkZ = escaped["chunkZ"].As<int>()
			});
		}
		UpdateChunks(new Vector2I(playerChunkX, playerChunkZ));
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsAction("save_and_exit"))
		{
			Save();
			Router.Of(this).Pop();
		}
	}
}
