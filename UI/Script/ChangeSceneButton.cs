using Godot;
using System;

public partial class ChangeSceneButton : Button
{
  [Export]
	private string scenePath;

  public override void _Pressed()
  {
	  GetTree().ChangeSceneToFile(scenePath);
  }
}
