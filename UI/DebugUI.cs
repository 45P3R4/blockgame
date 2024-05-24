using Godot;
using System;

public partial class DebugUI : Control
{
	private Label label;

    public override void _Ready()
    {
        label = GetChild<Label>(0);
    }

    public override void _Process(double delta)
	{
		label.Text = "FPS: " + Engine.GetFramesPerSecond();
	}
}
