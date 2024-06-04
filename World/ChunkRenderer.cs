using Godot;

public partial class ChunkRenderer : Node3D
{
	public static int RenderDistance = 6;
	public static Chunk[,] chunk = new Chunk[RenderDistance, RenderDistance];

	private Node3D player;

	PackedScene chunkScene = GD.Load<PackedScene>("res://Chunk/chunk.tscn");

	public override void _Ready()
	{
		

		for (int x = 0; x < RenderDistance; x++)
			for (int z = 0; z < RenderDistance; z++)
			{
				chunk[x,z] = chunkScene.Instantiate<Chunk>();
				chunk[x,z].Offset = new Vector3I(x*16,0,z*16);
				chunk[x,z].Init();
				chunk[x,z].Position = new Vector3I(x*16,0,z*16);
				AddChild(chunk[x,z]);
			}
			
		for (int x = 0; x < RenderDistance; x++)
			for (int z = 0; z < RenderDistance; z++)
			{
				chunk[x,z].GenerateChunkMesh();
			}
	}
}
