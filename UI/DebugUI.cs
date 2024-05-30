using Godot;
using System;

public partial class DebugUI : Control
{
	private Label label;
    private Node3D player;
    private TextureRect texture;

    public override void _Ready()
    {
        player = GetParent<Node3D>();
        label = GetChild<Label>(0);
        texture = GetChild<TextureRect>(1);
    }

    public override void _Process(double delta)
	{
		label.Text = "FPS: " + Engine.GetFramesPerSecond();
        label.Text += "\nPosition: X: " + (int)player.Position.X + " Y: " + (int)player.Position.Y + " Z: " + (int)player.Position.Z;
        texture.Texture = TextureBuilder.atlas;
	}
}
