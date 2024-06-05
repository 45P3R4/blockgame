using Godot;
using System;

public partial class ExitToMenuButton : Button
{
	[Export]
	private string scenePath;
	[Export]
	private Node root;

	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Always;
	}

	public override void _Pressed()
	{
		GetTree().ChangeSceneToFile(scenePath);
		GetTree().Paused = false;
		root.QueueFree();
	}
}
