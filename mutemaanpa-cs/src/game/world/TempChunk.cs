using System;
using Godot;

namespace Mutemaanpa;

public partial class TempChunk : Node3D
{
	[Export]
	private MeshInstance3D? meshInstance;

	private StandardMaterial3D? material;

	private Color GetColor()
	{
		var chunkCoords = World.ToChunkCoordinate(GlobalPosition.X, GlobalPosition.Z);
		var xy = chunkCoords.X * chunkCoords.Y;
		return Color.FromHsv(xy % 199 / 199.0f, 0.5f, 0.5f);
	}

	public override void _Ready()
	{
		material = new StandardMaterial3D();
		meshInstance!.MaterialOverride = material;
	}

	public override void _Process(double delta)
	{
		material!.AlbedoColor = GetColor();
	}
}
