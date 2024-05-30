using Godot;
using System;
using System.IO;
using System.Linq;

public partial class TextureBuilder : Node
{
	public static TextureBuilder textureBuilder;

	public static Texture2D atlas;

	public static int resolution = 32;
	public static int count = 16;

	private Image img;
	private Texture2D texture;
	

	public override void _Ready()
	{	
		img = Image.Create(count * resolution, resolution, false, Image.Format.Rgb8);
		DirAccess dir = DirAccess.Open("res://Assets/Textures/");
		if (dir.DirExists("res://Assets/Textures/"))
		{
			dir.ListDirBegin();
			string name = dir.GetNext();
			int i = 0;

			while (name != "")
			{
				if (!name.Contains(".import") && name.Contains(".png"))
				{
					//img.Resize(i*resolution, resolution, Image.Interpolation.Nearest);
					texture = GD.Load<Texture2D>("res://Assets/Textures/" + name);
					img.BlitRect(texture.GetImage(), new Rect2I(0,0,resolution,resolution), new Vector2I(i*32, 0));
					i++;
				}
				name = dir.GetNext();
			}
			
			atlas = ImageTexture.CreateFromImage(img);
			//GD.Load<Texture2D>("res://Assets/Textures/" + name);
			
		}
	}
}
