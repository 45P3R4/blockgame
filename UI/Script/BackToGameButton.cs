using Godot;
using System;

public partial class BackToGameButton : Button
{
    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
    }

    public override void _Pressed()
    {
		Input.MouseMode = Input.MouseModeEnum.Captured;
		GetTree().Paused = false;
		Node menuScene = GetTree().Root.GetNode<Node>("GameMenu");
        GetTree().Root.RemoveChild(menuScene);
    }
}
