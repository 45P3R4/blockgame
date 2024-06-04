using System.Collections.Generic;
using Godot;

public partial class Chunk : StaticBody3D
{
	public enum BlockType {
		Air = 0,
		Stone = 1,
		Dirt = 2,
		Cobblestone = 3
	};

	public Vector3I Offset = Vector3I.Zero;
	public const int width = 16;
	public const int height = 32;

	private MeshInstance3D instance;
	private NoiseTexture2D noise; //SEPARATE
	private Material material;
	private SurfaceTool st = new SurfaceTool();

	public int[,,] chunkData = new int[width, height, width];

    public void Init()
	{
		material = GD.Load<Material>("res://Chunk/chunk.tres");
		material.Set("shader_parameter/tex", TextureBuilder.atlas);
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
	}

	public void SpawnBlock(Vector3I position)
	{
		
		chunkData[position.X,position.Y,position.Z] = (int)BlockType.Dirt;
		GenerateChunkMesh();
		ChunkRenderer.chunk[Offset.X/16, Offset.Z/16+1].GenerateChunkMesh();
		ChunkRenderer.chunk[Offset.X/16+1, Offset.Z/16].GenerateChunkMesh();
		ChunkRenderer.chunk[Offset.X/16, Offset.Z/16-1].GenerateChunkMesh();
		ChunkRenderer.chunk[Offset.X/16-1, Offset.Z/16].GenerateChunkMesh();
	}

	public void BreakBlock(Vector3I position)
	{
		chunkData[position.X,position.Y,position.Z] = 0;
		GenerateChunkMesh();
		ChunkRenderer.chunk[Offset.X/16, Offset.Z/16+1].GenerateChunkMesh();
		ChunkRenderer.chunk[Offset.X/16+1, Offset.Z/16].GenerateChunkMesh();
		ChunkRenderer.chunk[Offset.X/16, Offset.Z/16-1].GenerateChunkMesh();
		ChunkRenderer.chunk[Offset.X/16-1, Offset.Z/16].GenerateChunkMesh();
	}

	public void GenerateChunkData()
	{
		noise.Noise.Set("Offset", new Vector3(Offset.X, Offset.Z, 0));

		for (int x = 0; x < width; x++)
			for (int y = 0; y < height; y++)
				for (int z = 0; z < width; z++)
				{
					if (y <= 10 + Mathf.FloorToInt(noise.Noise.GetNoise2D(x,z) * 10))
					{
						chunkData[x,y,z] = (int)BlockType.Stone;
					}
					else if (y <= 16 + Mathf.FloorToInt(noise.Noise.GetNoise2D(x,z) * 10))
					{
						chunkData[x,y,z] = (int)BlockType.Dirt;
					}
						
					else
						chunkData[x,y,z] = (int)BlockType.Air;
				}
	}

	public void GenerateChunkMesh() 
	{
		st.Begin(Mesh.PrimitiveType.Triangles);

		for (int x = 0; x < width; x++)
			for (int y = 0; y < height; y++)
				for (int z = 0; z < width; z++)
				{
					generateBlock(new Vector3I(x, y, z));
				}

		if(GetChild(0).GetChildCount() > 0)
			GetChild(0).GetChild(0).QueueFree();

		instance.Mesh = st.Commit();
		instance.CreateTrimeshCollision();
		instance.Mesh.SurfaceSetMaterial(0, material);
	}

	private void generateBlock(Vector3I position)
	{
		int x = position.X;
		int y = position.Y;
		int z = position.Z;

		if(getBlock(new Vector3I(x, y, z)) == 0)
			return;

		if (getBlock(new Vector3I(x, y, z-1)) == 0)
			generateFront(new Vector3I(x, y, z), (BlockType)chunkData[x,y,z]);

		if (getBlock(new Vector3I(x, y, z+1)) == 0)
			generateBack(new Vector3I(x, y, z), (BlockType)chunkData[x,y,z]);

		if (getBlock(new Vector3I(x, y-1, z)) == 0)
			generateBottom(new Vector3I(x, y, z), (BlockType)chunkData[x,y,z]);

		if (getBlock(new Vector3I(x, y+1, z)) == 0)
			generateTop(new Vector3I(x, y, z), (BlockType)chunkData[x,y,z]);

		if (getBlock(new Vector3I(x-1, y, z)) == 0)
			generateLeft(new Vector3I(x, y, z), (BlockType)chunkData[x,y,z]);

		if (getBlock(new Vector3I(x+1, y, z)) == 0)
			generateRight(new Vector3I(x, y, z), (BlockType)chunkData[x,y,z]);
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
		{
			if(position.Z < 0 && Offset.Z/16 > 0)
			{
				return ChunkRenderer.chunk[Offset.X/16, Offset.Z/16-1].
				chunkData[position.X, position.Y, position.Z+width];
			}
			if(position.Z >= width && Offset.Z/16 < ChunkRenderer.RenderDistance-1)
			{
				return ChunkRenderer.chunk[Offset.X/16, Offset.Z/16+1].
				chunkData[position.X, position.Y, position.Z-width];
			}

			if(position.X < 0 && Offset.X/16 > 0)
			{
				return ChunkRenderer.chunk[Offset.X/16-1, Offset.Z/16].
				chunkData[position.X+width, position.Y, position.Z];
			}
			if(position.X >= width && Offset.Z/16 < ChunkRenderer.RenderDistance-1)
			{
				return ChunkRenderer.chunk[Offset.X/16+1, Offset.Z/16].
				chunkData[position.X-width, position.Y, position.Z];
			}
		}
		return 1;
	}

	private Vector2 setTextureUV(int x, int y, BlockType blockType)
	{
		Vector2 uv = new Vector2(x, y);
		if (x <= 0)
			uv.X = ((float)blockType - 1) / 16;
		if(x > 0)
			uv.X = (float)blockType / 16;

		return uv;
	}

	private void generateBack(Vector3I position, BlockType blockType)
	{
		st.SetNormal(Vector3.Back);
		st.SetUV(setTextureUV(0, 0, blockType));
		st.AddVertex(new Vector3(0, 0, 1) + position);
		st.SetUV(setTextureUV(0, 1, blockType));
		st.AddVertex(new Vector3(0, 1, 1) + position);
		st.SetUV(setTextureUV(1, 1, blockType));
		st.AddVertex(new Vector3(1, 1, 1) + position);

		st.SetUV(setTextureUV(0, 0, blockType));
		st.AddVertex(new Vector3(0, 0, 1) + position);
		st.SetUV(setTextureUV(1, 1, blockType));
		st.AddVertex(new Vector3(1, 1, 1) + position);
		st.SetUV(setTextureUV(1, 0, blockType));
		st.AddVertex(new Vector3(1, 0, 1) + position);
	}

	private void generateFront(Vector3I position, BlockType blockType)
	{
		st.SetNormal(Vector3.Forward);
		st.SetUV(setTextureUV(0, 0, blockType));
		st.AddVertex(new Vector3(1, 0, 0) + position);
		st.SetUV(setTextureUV(0, 1, blockType));
		st.AddVertex(new Vector3(1, 1, 0) + position);
		st.SetUV(setTextureUV(1, 1, blockType));
		st.AddVertex(new Vector3(0, 1, 0) + position);
		
		st.SetUV(setTextureUV(0, 0, blockType));
		st.AddVertex(new Vector3(1, 0, 0) + position);
		st.SetUV(setTextureUV(1, 1, blockType));
		st.AddVertex(new Vector3(0, 1, 0) + position);
		st.SetUV(setTextureUV(1, 0, blockType));
		st.AddVertex(new Vector3(0, 0, 0) + position);
	}

	private void generateRight(Vector3I position, BlockType blockType)
	{
		st.SetNormal(Vector3.Right);
		st.SetUV(setTextureUV(0, 0, blockType));
		st.AddVertex(new Vector3(1, 0, 0) + position);
		st.SetUV(setTextureUV(1, 1, blockType));
		st.AddVertex(new Vector3(1, 1, 1) + position);
		st.SetUV(setTextureUV(0, 1, blockType));
		st.AddVertex(new Vector3(1, 1, 0) + position);
		
		
		st.SetUV(setTextureUV(0, 0, blockType));
		st.AddVertex(new Vector3(1, 0, 0) + position);
		st.SetUV(setTextureUV(1, 0, blockType));
		st.AddVertex(new Vector3(1, 0, 1) + position);
		st.SetUV(setTextureUV(1, 1, blockType));
		st.AddVertex(new Vector3(1, 1, 1) + position);
		
	}

	private void generateLeft(Vector3I position, BlockType blockType)
	{
		st.SetNormal(Vector3.Left);
		st.SetUV(setTextureUV(0, 0, blockType));
		st.AddVertex(new Vector3(0, 0, 1) + position);
		st.SetUV(setTextureUV(1, 1, blockType));
		st.AddVertex(new Vector3(0, 1, 0) + position);
		st.SetUV(setTextureUV(0, 1, blockType));
		st.AddVertex(new Vector3(0, 1, 1) + position);
		
		
		st.SetUV(setTextureUV(0, 0, blockType));
		st.AddVertex(new Vector3(0, 0, 1) + position);
		st.SetUV(setTextureUV(1, 0, blockType));
		st.AddVertex(new Vector3(0, 0, 0) + position);
		st.SetUV(setTextureUV(1, 1, blockType));
		st.AddVertex(new Vector3(0, 1, 0) + position);
		
	}

	private void generateTop(Vector3I position, BlockType blockType)
	{
		st.SetNormal(Vector3.Up);
		st.SetUV(setTextureUV(0, 0, blockType));
		st.AddVertex(new Vector3(0, 1, 1) + position);
		st.SetUV(setTextureUV(0, 1, blockType));
		st.AddVertex(new Vector3(0, 1, 0) + position);
		st.SetUV(setTextureUV(1, 1, blockType));
		st.AddVertex(new Vector3(1, 1, 0) + position);
		
		st.SetUV(setTextureUV(0, 0, blockType));
		st.AddVertex(new Vector3(0, 1, 1) + position);
		st.SetUV(setTextureUV(1, 1, blockType));
		st.AddVertex(new Vector3(1, 1, 0) + position);
		st.SetUV(setTextureUV(1, 0, blockType));
		st.AddVertex(new Vector3(1, 1, 1) + position);
	}

	private void generateBottom(Vector3I position, BlockType blockType)
	{
		st.SetNormal(Vector3.Down);
		st.SetUV(setTextureUV(0, 0, blockType));
		st.AddVertex(new Vector3(0, 0, 0) + position);
		st.SetUV(setTextureUV(0, 1, blockType));
		st.AddVertex(new Vector3(0, 0, 1) + position);
		st.SetUV(setTextureUV(1, 1, blockType));
		st.AddVertex(new Vector3(1, 0, 1) + position);
		
		st.SetUV(setTextureUV(0, 0, blockType));
		st.AddVertex(new Vector3(0, 0, 0) + position);
		st.SetUV(setTextureUV(1, 1, blockType));
		st.AddVertex(new Vector3(1, 0, 1) + position);
		st.SetUV(setTextureUV(1, 0, blockType));
		st.AddVertex(new Vector3(1, 0, 0) + position);
	}
}
