using Godot;
using System;

public partial class World : Node3D
{
	PackedScene chunkScene = GD.Load<PackedScene>("res://chunk.tscn");

	public override void _Ready()
	{
		Chunk ch = chunkScene.Instantiate<Chunk>();

		for (int x = 0; x < 16; x++)
			for (int z = 0; z < 16; z++)
			{
				ch = chunkScene.Instantiate<Chunk>();
				ch.Offset = new Vector3I(x*16,0,z*16);
				ch.Init();
				AddChild(ch);
			}
	}
}
