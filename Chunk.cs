using Godot;

[Tool]
public partial class Chunk : StaticBody3D
{
	public enum BlockType {
		Air = 0,
		Stone = 1
	};

	public Vector3I Offset = Vector3I.Zero;
	public const int width = 16;
	public const int height = 32;

	private MeshInstance3D instance;
	private NoiseTexture2D noise;
	private SurfaceTool st = new SurfaceTool();

	private int[,,] chunkData = new int[width, height, width];

    public void Init()
	{
		instance = GetChild<MeshInstance3D>(0);
        noise = new NoiseTexture2D
        {
            Width = 16,
			Height = 16,
			Noise = new FastNoiseLite {
				Offset = new Vector3(Offset.X, Offset.Z, 0)
			}
        };

        GenerateChunkData();
		GenerateChunk();	
	}

	public void SpawnBlock(Vector3I position)
	{
		chunkData[position.X,position.Y,position.Z] = 1;
		GenerateChunk();
	}

	public void BreakBlock(Vector3I position)
	{
		chunkData[position.X,position.Y,position.Z] = 0;
		GenerateChunk();
	}

	public void GenerateChunk() 
	{
		st.Begin(Mesh.PrimitiveType.Triangles);

		for (int x = 0; x < width; x++)
			for (int z = 0; z < width; z++)
				for (int y = 0; y < height; y++)
				{
					generateBlock(x, y, z);
				}

		instance.Mesh = st.Commit();
		if(GetChild(0).GetChildCount() > 0)
			GetChild(0).GetChild(0).QueueFree();
		instance.CreateTrimeshCollision();
	}

	private void GenerateChunkData()
	{
		noise.Noise.Set("Offset", new Vector3(Offset.X, Offset.Y, 0));

		for (int x = 0; x < width; x++)
			for (int z = 0; z < width; z++)
				for (int y = 0; y < height; y++)
				{
					if (y <= 16 + Mathf.FloorToInt(noise.Noise.GetNoise2D(x,z) * 10))
						chunkData[x,y,z] = 1;
					else
						chunkData[x,y,z] = 0;
				}
	}

	private void generateBlock(int x, int y, int z)
	{
		if(getBlock(new Vector3I(x, y, z)) == 0)
			return;

		if (getBlock(new Vector3I(x, y, z-1)) == 0)
			generateFront(new Vector3I(Offset.X + x, y, Offset.Z + z));

		if (getBlock(new Vector3I(x, y, z+1)) == 0)
			generateBack(new Vector3I(Offset.X + x, y, Offset.Z + z));

		if (getBlock(new Vector3I(x, y-1, z)) == 0)
			generateBottom(new Vector3I(Offset.X + x, y, Offset.Z + z));

		if (getBlock(new Vector3I(x, y+1, z)) == 0)
			generateTop(new Vector3I(Offset.X + x, y, Offset.Z + z));

		if (getBlock(new Vector3I(x-1, y, z)) == 0)
			generateLeft(new Vector3I(Offset.X + x, y, Offset.Z + z));

		if (getBlock(new Vector3I(x+1, y, z)) == 0)
			generateRight(new Vector3I(Offset.X + x, y, Offset.Z + z));
	}

	private int getBlock(Vector3I position)
	{
		if (position.X >= 0 && position.X < width &&
			position.Y >= 0 && position.Y < height &&
			position.Z >= 0 && position.Z < width)
			{
				return chunkData[position.X, position.Y, position.Z];
			}
		else
			return 1;
	}

	private void generateBack(Vector3I position)
	{
		st.SetNormal(Vector3.Back);
		st.SetUV(new Vector2(0,0));
		st.AddVertex(new Vector3(0, 0, 0) + position);
		st.SetUV(new Vector2(0,1));
		st.AddVertex(new Vector3(0, 1, 0) + position);
		st.SetUV(new Vector2(1,1));
		st.AddVertex(new Vector3(1, 1, 0) + position);

		st.SetUV(new Vector2(0,0));
		st.AddVertex(new Vector3(0, 0, 0) + position);
		st.SetUV(new Vector2(1,1));
		st.AddVertex(new Vector3(1, 1, 0) + position);
		st.SetUV(new Vector2(1,0));
		st.AddVertex(new Vector3(1, 0, 0) + position);
	}

	private void generateFront(Vector3I position)
	{
		st.SetNormal(Vector3.Forward);
		st.SetUV(new Vector2(0,0));
		st.AddVertex(new Vector3(1, 0, -1) + position);
		st.SetUV(new Vector2(0,1));
		st.AddVertex(new Vector3(1, 1, -1) + position);
		st.SetUV(new Vector2(1,1));
		st.AddVertex(new Vector3(0, 1, -1) + position);
		
		st.SetUV(new Vector2(0,0));
		st.AddVertex(new Vector3(1, 0, -1) + position);
		st.SetUV(new Vector2(1,1));
		st.AddVertex(new Vector3(0, 1, -1) + position);
		st.SetUV(new Vector2(1,0));
		st.AddVertex(new Vector3(0, 0, -1) + position);
	}

	private void generateRight(Vector3I position)
	{
		st.SetNormal(Vector3.Left);
		st.SetUV(new Vector2(0,0));
		st.AddVertex(new Vector3(1, 0, -1) + position);
		st.SetUV(new Vector2(1,1));
		st.AddVertex(new Vector3(1, 1, 0) + position);
		st.SetUV(new Vector2(0,1));
		st.AddVertex(new Vector3(1, 1, -1) + position);
		
		
		st.SetUV(new Vector2(0,0));
		st.AddVertex(new Vector3(1, 0, -1) + position);
		st.SetUV(new Vector2(1,0));
		st.AddVertex(new Vector3(1, 0, 0) + position);
		st.SetUV(new Vector2(1,1));
		st.AddVertex(new Vector3(1, 1, 0) + position);
		
	}

	private void generateLeft(Vector3I position)
	{
		st.SetNormal(Vector3.Right);
		st.SetUV(new Vector2(0,0));
		st.AddVertex(new Vector3(0, 0, 0) + position);
		st.SetUV(new Vector2(1,1));
		st.AddVertex(new Vector3(0, 1, -1) + position);
		st.SetUV(new Vector2(0,1));
		st.AddVertex(new Vector3(0, 1, 0) + position);
		
		
		st.SetUV(new Vector2(0,0));
		st.AddVertex(new Vector3(0, 0, 0) + position);
		st.SetUV(new Vector2(1,0));
		st.AddVertex(new Vector3(0, 0, -1) + position);
		st.SetUV(new Vector2(1,1));
		st.AddVertex(new Vector3(0, 1, -1) + position);
		
	}

	private void generateTop(Vector3I position)
	{
		st.SetNormal(Vector3.Up);
		st.SetUV(new Vector2(0,0));
		st.AddVertex(new Vector3(0, 1, 0) + position);
		st.SetUV(new Vector2(0,1));
		st.AddVertex(new Vector3(0, 1, -1) + position);
		st.SetUV(new Vector2(1,1));
		st.AddVertex(new Vector3(1, 1, -1) + position);
		
		st.SetUV(new Vector2(0,0));
		st.AddVertex(new Vector3(0, 1, 0) + position);
		st.SetUV(new Vector2(1,1));
		st.AddVertex(new Vector3(1, 1, -1) + position);
		st.SetUV(new Vector2(1,0));
		st.AddVertex(new Vector3(1, 1, 0) + position);
	}

	private void generateBottom(Vector3I position)
	{
		st.SetNormal(Vector3.Down);
		st.SetUV(new Vector2(0,0));
		st.AddVertex(new Vector3(0, 0, -1) + position);
		st.SetUV(new Vector2(0,1));
		st.AddVertex(new Vector3(0, 0, 0) + position);
		st.SetUV(new Vector2(1,1));
		st.AddVertex(new Vector3(1, 0, 0) + position);
		
		st.SetUV(new Vector2(0,0));
		st.AddVertex(new Vector3(0, 0, -1) + position);
		st.SetUV(new Vector2(1,1));
		st.AddVertex(new Vector3(1, 0, 0) + position);
		st.SetUV(new Vector2(1,0));
		st.AddVertex(new Vector3(1, 0, -1) + position);
	}
}
